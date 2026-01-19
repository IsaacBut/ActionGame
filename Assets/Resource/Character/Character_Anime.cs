using UnityEngine;
using System.Collections;

/// <summary>
/// キ??クターのアニ?ーシ??関連??を管?する基底ク?ス。
/// 攻?コ?ボ、回避（?ー?）、移動、アニ?ーシ??再生制御など、
/// 多くのキ??クター共通?ジックをまとめています。
/// </summary>
public class Character_Anime : Character
{
    /// <summary>
    /// 攻?コ?ボの種類を表す列?体。
    /// Combo01 ～ Combo04 まで存在し、ComboCount はコ?ボ??として?用します。
    /// </summary>
    public enum AttackComboType
    {
        Combo01 = 1, Combo02 = 2, Combo03 = 3, Combo04 = 4, ComboCount
    }
    /// <summary>
    /// 現在の攻?コ?ボ段階。
    /// 攻?間隔が一定?間を超えると Combo01 に?セットされます。
    /// </summary>
    [SerializeField] public AttackComboType attackComboType;
    /// <summary>
    /// キ??クターのアニ?ーターコ?ポーネ?ト。
    /// </summary>
    public Animator animator { get; set; }
    /// <summary>
    /// 戦闘状態フ?グ。
    /// </summary>
    public bool battle;
    /// <summary>
    /// 通常状態フ?グ。
    /// </summary>
    public bool normal;
    /// <summary>
    /// 移動状態フ?グ。
    /// </summary>
    public bool move;
    /// <summary>
    /// 攻?可能かどうかを示すフ?グ。
    /// </summary>
    public bool canAttack = true;
    /// <summary>
    /// ?ー?（回避）可能かどうかを示すフ?グ。
    /// </summary>
    public bool canRool = true;

    /// <summary>
    /// アニ?ーシ??関連の?期化??。
    /// 派生ク?スで必要に?じて上?きします。
    /// </summary>
    public virtual void AnimeInit() { }
    /// <summary>
    /// キ??クターの経過?間（基底ク?スの characterTimer を使用）。
    /// </summary>
    public float gameTimer => characterTimer;
    /// <summary>
    /// 最後に?ー?を行った?間。
    /// </summary>
    public float rollTime;
    /// <summary>
    /// 最後に攻?を行った?間。
    /// </summary>
    public float attackTime;
    /// <summary>
    /// コ?ボ?セットに必要なクー?ダウ?秒?。
    /// </summary>
    private const float attackCd = 2.0f;
    /// <summary>
    /// 最大コ?ボ?（AttackComboType.ComboCount を整?化したもの）。
    /// </summary>
    public int maxCombo = (int)AttackComboType.ComboCount;
    /// <summary>
    /// ?ー?可能になるまでのクー?ダウ??間。
    /// </summary>
    private const float rollCD = 2.0f;

    /// <summary>
    /// 攻?・回避のクー?ダウ???を管?する。
    /// ・?ー?：前回から一定?間経過で canRool が true  
    /// ・攻? ：一定?間経過でコ?ボを Combo01 に戻す
    /// </summary>
    public virtual void AnimeTimer()
    {
        canRool = (gameTimer - rollTime) >= rollCD ? true : false;
        attackComboType = (gameTimer - attackTime) >= attackCd ? AttackComboType.Combo01 : attackComboType;
    }

    /// <summary>
    /// 指定したアニ?ーシ??ステートが再生され、さらに normalizedTime が
    /// 指定?間に達するまで待機するコ?ーチ?。
    /// </summary>
    /// <param name="stateName">アニ?ーシ??ステート名</param>
    /// <param name="time">normalizedTime の目標値（例：1.0f で全再生）</param>
    public IEnumerator WaitForAnimation(string stateName, float time)
    {
        // 指定ステートに遷移するまで待機
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        // ステートの再生が一定?間以上になるまで待機
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= time);

        Debug.Log(stateName + "Animation Finished!");
    }

    /// <summary>
    /// 移動??。派生ク?スで上?きします。
    /// </summary>
    public virtual void Move() { }
    /// <summary>
    /// 停止??。派生ク?スで上?きします。
    /// </summary>
    public virtual void Stop() { }
    /// <summary>
    /// 攻?クー?ダウ?用コ?ーチ?イ?スタ?ス。
    /// </summary>
    public Coroutine attackCdCoroutine;
    /// <summary>
    /// マウス入力受付の最小間隔。
    /// AttackCD() 内の待機に使用。
    /// </summary>
    public const float mouseInputCd = 0.1f;
    /// <summary>
    /// 攻?クー?ダウ?（入力受付制限）を管?するコ?ーチ?。
    /// 指定秒?待機後に canAttack を true に戻す。
    /// </summary>
    public IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(mouseInputCd);
        canAttack = true;
        attackCdCoroutine = null;
    }
    /// <summary>
    /// 現在のコ?ボ段階に?じたアニ?ーシ??ク?ップ名を返す。
    /// 例：Attack_Combo_01, Attack_Combo_02 …
    /// </summary>
    public string AttackAnimeCrip() => $"Attack_Combo_0{(int)attackComboType}";
    /// <summary>
    /// 攻?アニ?ーシ????。
    /// 派生ク?スで実?します。
    /// </summary>
    public virtual IEnumerator Attack() { yield return 0; }

    /// <summary>
    /// ?ー?（回避）アニ?ーシ????。
    /// 派生ク?スで実?します。
    /// </summary>
    public virtual IEnumerator Rool() { yield return 0; }


}
