using System.Collections.Generic;
using UnityEngine;
using static SkillBuff;

/// <summary>
/// キャラクターのステータス・装備・バフ処理を管理するクラス。
/// タイマー更新、バフ付与・解除、ステータス再計算などを担当する。
/// </summary>
public class Character : MonoBehaviour
{
    /// <summary>キャラクターが生存している累計時間（秒）</summary>
    public float characterTimer;

    /// <summary>
    /// キャラクターのステージ（行動状態）を表す列挙体。  
    /// ・Idle   … 待機状態  
    /// ・Move   … 移動状態  
    /// ・Defend … 防御状態  
    /// ・Attack … 攻撃状態  
    /// ・Dead   … 戦闘不能状態
    /// </summary>
    public enum CharacterStage
    {
        Idle = 0,
        Move,
        Defend,
        Attack,
        Dead
    }
    /// <summary>
    /// 現在のキャラクターステージ。
    /// 行動状態の管理に使用されます。
    /// </summary>
    public CharacterStage nowStage;
    /// <summary>
    /// ステージを変更する処理。  
    /// ※現在の仕様では「より大きいステージ番号へ進む場合のみ変更可能」。  
    /// 　例：Idle → Move（可能）  
    ///   　　Attack → Move（不可）  
    /// </summary>
    /// <param name="targetStage">遷移を試みるステージ</param>
    /// <returns>
    /// ステージ変更が行われた場合 true、  
    /// 変更できない場合 false を返します。
    /// </returns>
    public bool ChangeStage(CharacterStage targetStage)
    {
        var isStageChange = nowStage > targetStage ? false : true;
        nowStage = isStageChange ? targetStage : nowStage;
        return isStageChange;
    }
    /// <summary>
    /// 最終入力（移動や攻撃など）から Idle に戻るまでの時間。
    /// </summary>
    public const float returnIdleTime = 0.5f;
    /// <summary>
    /// 最後に入力が行われたタイミングを記録する変数。  
    /// characterTimer と併用して Idle 戻り処理に利用。
    /// </summary>
    public float lastInputTime;
    /// <summary>
    /// 入力が一定時間無かった場合、自動的に Idle ステージへ戻す処理。  
    /// ・現在が Idle なら何もしない  
    /// ・lastInputTime から returnIdleTime を超えたら Idle に戻す
    /// </summary>
    public void ReturnIdle()
    {
        if (nowStage == CharacterStage.Idle) return;

        if (characterTimer - lastInputTime > returnIdleTime)
        {
            nowStage = CharacterStage.Idle;
        }
    }
    /// <summary>
    /// キャラクター固有タイマーを更新する。
    /// Update / FixedUpdate から呼び出して使用する。
    /// </summary>
    public void Timer() => characterTimer += Time.deltaTime;
    /// <summary>右手武器の攻撃属性</summary>
    public AttackElements rightHandElements;
    /// <summary>左手武器の攻撃属性</summary>
    public AttackElements leftHandElements;


    /// <summary>現在付与されているバフの集合</summary>
    public HashSet<Buff> buffs = new HashSet<Buff>();
    /// <summary>バフとその終了時間(characterTimerベース)のマッピング</summary>
    private Dictionary<Buff, float> buffsExistTime = new Dictionary<Buff, float>();

    public int buffNumber;

    [Header("Player Status")]
    /// <summary>HP の最大値（バフ適用前の基礎値）</summary>
    private int maxHp {  get; set; }
    /// <summary>MP の最大値（バフ適用前の基礎値）</summary>
    private int maxMp { get; set; }
    /// <summary>STR（攻撃力など）の基礎値</summary>
    private int baseStr { get; set; }
    /// <summary>現在の HP（毎フレーム更新される可能性あり）</summary>
    private int hp { get; set; }
    /// <summary>現在の MP（毎フレーム更新される可能性あり）</summary>
    private int mp { get; set; }
    /// <summary>現在の STR。基礎値 + バフ + 武器補正が反映される</summary>
    private int str { get; set; }

    public float moveSpeed { get;  private set; }

    // ---- バフ計算用の一時値 ----
    /// <summary>バフによって増加した HP の合計値</summary>
    private float buffhp = 0;
    /// <summary>バフによって増加した MP の合計値</summary>
    private float buffmp = 0;
    /// <summary>バフによって増加した STR の合計値</summary>
    private float buffstr = 0;

    // ---- 装備情報 ----
    /// <summary>左手に装備している武器（Wepon クラス）</summary>
    public Wepon leftHand;
    /// <summary>右手に装備している武器（Wepon クラス）</summary>
    public Wepon rightHand;

    /// <summary>
    /// キャラクターの基礎ステータスおよびバフ管理用のコレクションを初期化する。
    /// キャラ生成時やゲーム開始時に 1 回だけ呼び出す想定。
    /// </summary>
    public void StatusInit(int Hp,int Mp,int Str)
    {
        maxHp = Hp; maxMp = Mp; baseStr = Str;
        // 現在値を最大値で初期化
        hp = maxHp; mp = maxMp; str = Str;
        // バフ管理セットを初期化
        moveSpeed = 1.0f;
    }

    public virtual void CharacterInit() { } 

    /// <summary>
    /// キャラクターの現在ステータスを再計算する。
    /// ・ValueBuff（ステータス系バフ）
    /// ・右手武器の STR 補正  
    /// を反映する。
    /// </summary>
    public void CharacterStatus()
    {
        buffNumber = buffs.Count;
        // ステータス系バフの合計値を取得
        ValueBuff_Value(out buffhp, out buffmp, out buffstr);
        // HP → バフ加算後、最大値を超えないように調整
        hp = hp + (int)buffhp > maxHp ? maxHp : hp + (int)buffhp;
        // MP → バフ加算後、最大値を超えないように調整
        mp = mp + (int)buffmp > maxMp ? maxMp : mp + (int)buffmp;
        // STR → 基礎 STR + バフ + 武器の STR 補正
        str = baseStr;
        str += (int)buffstr + rightHand.str;
    }
    /// <summary>
    /// 右手武器に付与されるスキルバフ（攻撃属性）を更新する。
    /// スキルバフは最後に適用された属性が優先される仕組み。
    /// </summary>
    public void RightWeponStatus()
    {
        if (rightHand == null) return;                  // 武器未装備時は処理しない
        SkillBuff_Skill(out rightHand.attackElements);  // スキルバフ（属性）を取得して武器に反映
        rightHandElements = rightHand.attackElements;   // キャラクター側の記録用変数にも代入

    }

    /// <summary>
    /// バフをキャラクターに追加する。
    /// ・同名バフが既に存在する場合はレベル比較を行う
    /// ・レベルが高いものに置き換える
    /// ・レベルが同等または低い場合は持続時間だけ延長する
    /// 新規バフはそのまま追加される。
    /// </summary>
    public void AddBuff(Buff newBuff)
    {
        // このバフが終了する予定の時間（現在の時間 + バフの持続時間）
        float existTime = characterTimer + newBuff.buffExistTime;
        foreach (Buff buff in buffs)
        {

            if (buff.name == newBuff.name)
            {
                // レベルが高ければ既存バフを置き換える
                if (newBuff.level > buff.level)
                {
                    buffs.Remove(buff);
                    buffsExistTime.Remove(buff);

                    buffs.Add(newBuff);
                    buffsExistTime.Add(newBuff, existTime);
                    return;
                }
                // レベルが同等または低い → 持続時間のみ更新
                buffsExistTime[buff] = existTime;
                return;
            }
        }
        // 新規バフを追加
        buffs.Add(newBuff);
        buffsExistTime.Add(newBuff, existTime);
    }
    /// <summary>
    /// 指定したバフを削除する。
    /// buff が存在しない場合は警告を出す。
    /// </summary>
    private void RemoveBuff(Buff buff)
    {
        // buffs から削除
        if (buffs.Contains(buff)) buffs.Remove(buff);
        else Debug.LogWarning($"Buff no exist in buffs : {buff.name}");
        // buffsExistTime から削除
        if (buffsExistTime.ContainsKey(buff)) buffsExistTime.Remove(buff);
        else Debug.LogWarning($"Buff no exist in buffsExistTime : {buff.name}");
    }

    /// <summary>
    /// バフの残り時間を監視し、期限切れのバフを削除する。
    /// FixedUpdate に入れて定期的に呼び出すことを想定。
    /// </summary>
    public void BuffTimer()
    {
        if (buffsExistTime.Count == 0) return;
        // 削除対象バフを一時的に保存するセット
        HashSet<Buff> toRemove = new HashSet<Buff>();

        // ※ 辞書を直接ループ中に削除するとエラーになるため一度集める
        foreach (Buff buff in buffsExistTime.Keys)
        {
            // 終了時間を過ぎていたら削除候補へ
            if (characterTimer > buffsExistTime[buff]) toRemove.Add(buff);
        }
        // 候補をまとめて削除
        foreach (var key in toRemove)
        {
            Debug.Log(key.name);
            RemoveBuff(key);
        }

    }
    /// <summary>
    /// すべての ValueBuff（ステータス系バフ）の合計値を計算する。
    /// スキル専用バフ (SkillOnly) は無視される。
    /// </summary>
    public void ValueBuff_Value(out float hp, out float mp, out float str)
    {
        float buffHp = 0;
        float buffMp = 0;
        float buffstr = 0;

        foreach (Buff buff in buffs)
        {
            // SkillOnly → ステータスには影響しない
            if (buff.type == Buff.BuffType.SkillOnly)
            {
               continue;
            }
            // 本来 ValueBuff があるはずのバフに null が入っていた場合
            if (buff.valueBuff == null)
            {
                Debug.LogWarning("ValueBuff Missing");
                continue;
            }
            // ステータスへの加算値を集計
            buffHp += buff.valueBuff.hp;
            buffMp += buff.valueBuff.mp;
            buffstr += buff.valueBuff.str;

        }
        hp = buffHp;
        mp = buffMp;
        str = buffstr;

    }

    /// <summary>
    /// すべてのスキルバフ（攻撃属性）の最終値を取得する。
    /// ・ValueOnly のバフは無視
    /// ・最後に適用された属性が優先される
    /// </summary>
    public void SkillBuff_Skill(out AttackElements elements)
    {

        AttackElements buffElement = AttackElements.None;
        foreach (Buff buff in buffs)
        {
            // ステータス専用バフは無視
            if (buff.type == Buff.BuffType.ValueOnly)
            {
                elements = buffElement; continue;
            }
            // 本来 SkillBuff があるべきなのに null の場合
            if (buff.skillBuff == null)
            {
                Debug.LogWarning("SkillBuff Missing");
                elements = buffElement; continue;
            }
            // 属性を更新（後勝ち方式）
            buffElement = buff.skillBuff.attackElements;
        }
        elements = buffElement;
    }

}
