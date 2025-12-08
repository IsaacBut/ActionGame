using UnityEngine;
using System.Collections;

/// <summary>
/// キャラクターのアニメーション関連処理を管理する基底クラス。
/// 攻撃コンボ、回避（ロール）、移動、アニメーション再生制御など、
/// 多くのキャラクター共通ロジックをまとめています。
/// </summary>
public class Character_Anime : Character
{
    /// <summary>
    /// 攻撃コンボの種類を表す列挙体。
    /// Combo01 ～ Combo04 まで存在し、ComboCount はコンボ総数として利用します。
    /// </summary>
    public enum AttackComboType
    {
        Combo01 = 1, Combo02 = 2, Combo03 = 3, Combo04 = 4, ComboCount
    }
    /// <summary>
    /// 現在の攻撃コンボ段階。
    /// 攻撃間隔が一定時間を超えると Combo01 にリセットされます。
    /// </summary>
    [SerializeField] public AttackComboType attackComboType;
    /// <summary>
    /// キャラクターのアニメーターコンポーネント。
    /// </summary>
    public Animator animator { get; set; }
    /// <summary>
    /// 戦闘状態フラグ。
    /// </summary>
    public bool battle;
    /// <summary>
    /// 通常状態フラグ。
    /// </summary>
    public bool normal;
    /// <summary>
    /// 移動状態フラグ。
    /// </summary>
    public bool move;
    /// <summary>
    /// 攻撃可能かどうかを示すフラグ。
    /// </summary>
    public bool canAttack = true;
    /// <summary>
    /// ロール（回避）可能かどうかを示すフラグ。
    /// </summary>
    public bool canRool = true;

    /// <summary>
    /// アニメーション関連の初期化処理。
    /// 派生クラスで必要に応じて上書きします。
    /// </summary>
    public virtual void AnimeInit() { }
    /// <summary>
    /// キャラクターの経過時間（基底クラスの characterTimer を使用）。
    /// </summary>
    public float gameTimer => characterTimer;
    /// <summary>
    /// 最後にロールを行った時間。
    /// </summary>
    public float rollTime;
    /// <summary>
    /// 最後に攻撃を行った時間。
    /// </summary>
    public float attackTime;
    /// <summary>
    /// コンボリセットに必要なクールダウン秒数。
    /// </summary>
    private const float attackCd = 2.0f;
    /// <summary>
    /// 最大コンボ数（AttackComboType.ComboCount を整数化したもの）。
    /// </summary>
    public int maxCombo = (int)AttackComboType.ComboCount;
    /// <summary>
    /// ロール可能になるまでのクールダウン時間。
    /// </summary>
    private const float rollCD = 2.0f;

    /// <summary>
    /// 攻撃・回避のクールダウン処理を管理する。
    /// ・ロール：前回から一定時間経過で canRool が true  
    /// ・攻撃 ：一定時間経過でコンボを Combo01 に戻す
    /// </summary>
    public virtual void AnimeTimer()
    {
        canRool = (gameTimer - rollTime) >= rollCD ? true : false;
        attackComboType = (gameTimer - attackTime) >= attackCd ? AttackComboType.Combo01 : attackComboType;
    }

    /// <summary>
    /// 指定したアニメーションステートが再生され、さらに normalizedTime が
    /// 指定時間に達するまで待機するコルーチン。
    /// </summary>
    /// <param name="stateName">アニメーションステート名</param>
    /// <param name="time">normalizedTime の目標値（例：1.0f で全再生）</param>
    public IEnumerator WaitForAnimation(string stateName, float time)
    {
        // 指定ステートに遷移するまで待機
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        // ステートの再生が一定時間以上になるまで待機
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= time);

        Debug.Log(stateName + "Animation Finished!");
    }

    /// <summary>
    /// 移動処理。派生クラスで上書きします。
    /// </summary>
    public virtual void Move() { }
    /// <summary>
    /// 停止処理。派生クラスで上書きします。
    /// </summary>
    public virtual void Stop() { }
    /// <summary>
    /// 攻撃クールダウン用コルーチンインスタンス。
    /// </summary>
    public Coroutine attackCdCoroutine;
    /// <summary>
    /// マウス入力受付の最小間隔。
    /// AttackCD() 内の待機に使用。
    /// </summary>
    public const float mouseInputCd = 0.1f;
    /// <summary>
    /// 攻撃クールダウン（入力受付制限）を管理するコルーチン。
    /// 指定秒数待機後に canAttack を true に戻す。
    /// </summary>
    public IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(mouseInputCd);
        canAttack = true;
        attackCdCoroutine = null;
    }
    /// <summary>
    /// 現在のコンボ段階に応じたアニメーションクリップ名を返す。
    /// 例：Attack_Combo_01, Attack_Combo_02 …
    /// </summary>
    public string AttackAnimeCrip() => $"Attack_Combo_0{(int)attackComboType}";
    /// <summary>
    /// 攻撃アニメーション処理。
    /// 派生クラスで実装します。
    /// </summary>
    public virtual IEnumerator Attack() { yield return 0; }

    /// <summary>
    /// ロール（回避）アニメーション処理。
    /// 派生クラスで実装します。
    /// </summary>
    public virtual IEnumerator Rool() { yield return 0; }


}
