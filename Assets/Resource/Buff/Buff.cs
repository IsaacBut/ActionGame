using UnityEngine;
using static SkillBuff;

/// <summary>
/// キャラクターに付与されるバフ情報を管理するクラス。
/// バフ名・レベル・持続時間・バフ種別に加えて、
/// ステータス強化用(ValueBuff) と スキル強化用(SkillBuff) のデータも保持する。
/// </summary>
public class Buff
{
    /// <summary>バフの名称</summary>
    public string name { get; private set; }
    /// <summary>バフのレベル</summary>
    public int level { get; private set; }
    /// <summary>バフの持続時間（秒）</summary>
    public float buffExistTime { get; private set; }
    /// <summary>
    /// バフの種類
    /// ValueOnly = ステータスのみ強化
    /// SkillOnly = スキル効果のみ強化
    /// Both      = 両方の効果を持つ
    /// </summary>
    [SerializeField] public enum BuffType
    {
        ValueOnly,
        SkillOnly,
        Both
    }
    /// <summary>バフの種類</summary>
    public BuffType type { get; private set; }
    /// <summary>ステータス強化用のバフデータ</summary>
    public ValueBuff valueBuff;
    /// <summary>スキル強化用のバフデータ</summary>
    public SkillBuff skillBuff;

    /// <summary>
    /// バフの基本情報を初期化する。
    /// バフ種別に応じて ValueBuff / SkillBuff を生成する。
    /// </summary>
    /// <param name="buffName">バフ名</param>
    /// <param name="buffLevel">バフレベル</param>
    /// <param name="Time">持続時間</param>
    /// <param name="buffType">バフの種類</param>
    /// </summary>
    public void BuffInit(string buffName, int buffLevel,float Time, BuffType buffType)
    {
        name = buffName; level = buffLevel; buffExistTime = Time; type = buffType;

        // バフ種別に応じて必要なバフデータクラスを生成
        switch (type)
        {
            case BuffType.ValueOnly:    // ステータス強化のみ
                valueBuff = new ValueBuff();
                break;

            case BuffType.SkillOnly:    // スキル強化のみ
                skillBuff = new SkillBuff();
                break;

            case BuffType.Both:         // 両方生成
                valueBuff = new ValueBuff();
                skillBuff = new SkillBuff();
            break;
        }
    }
    /// <summary>
    /// ValueBuff のステータス値を初期化する。
    /// ValueBuff が存在しない場合は警告を表示して処理しない。
    /// </summary>
    /// <param name="buffHp">HP上昇量</param>
    /// <param name="buffMp">MP上昇量</param>
    /// <param name="buffStr">STR上昇量</param>
    public void ValueBuffInit(float buffHp,float buffMp, float buffStr)
    {
        if (valueBuff == null)
        {
            // ValueBuff が存在しない場合
            Debug.LogWarning("Wrong Buff(ValueBuff) Init");
            return;
        }
        // ステータスバフ値をセット
        valueBuff.hp = buffHp;
        valueBuff.mp = buffMp;
        valueBuff.str = buffStr;

    }
    /// <summary>
    /// SkillBuff を初期化する。
    /// SkillBuff が存在しない場合は警告を表示して処理しない。
    /// </summary>
    /// <param name="elements">付与する攻撃属性</param>
    public void SkillBuffInit(AttackElements elements)
    {
        if (skillBuff == null)
        {
            // SkillBuff が存在しない場合
            Debug.LogWarning("Wrong Buff(SkillBuff) Init");
            return;
        }
        // スキルバフの属性を設定
        skillBuff.attackElements = elements;
    } 

}
