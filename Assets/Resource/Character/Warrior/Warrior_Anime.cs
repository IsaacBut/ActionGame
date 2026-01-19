using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// 戦士（Warrior）専用のアニ?ーシ??制御ク?ス。
/// Character_Anime を継承し、移動・停止・攻?・?ー?など
/// 戦士固有のアニ?ーシ???動を実?します。
/// </summary>
public class Warrior_Anime : Character_Anime
{
    /// <summary>
    /// アニ?ーシ??の?期化??。
    /// ・Animator の取得
    /// ・バト?／通常状態の?期設定
    /// ・コ?ボ?期化
    /// </summary>
    public override void AnimeInit()
    {
        animator = this.gameObject.GetComponent<Animator>();

        battle = true;
        normal = false;
        attackComboType = AttackComboType.Combo01;

        animator.SetBool("Battle", battle);
        animator.SetBool("Normal", normal);
        animator.SetBool("Move", move);

    }
    /// <summary>
    /// 移動開始?のアニ?ーシ??切り替え??。
    /// Move フ?グが false の場?に true へ変更し、
    /// Animator 側の "Move" パ??ータを更新します。
    /// </summary>
    public override void Move()
    {
        if (!move) move = true;
        animator.SetBool("Move", move);
    }
    /// <summary>
    /// 停止?のアニ?ーシ??切り替え??。
    /// Move フ?グが true の場?に false へ変更し、
    /// Animator 側の "Move" パ??ータを更新します。
    /// </summary>
    public override void Stop()
    {
        if (move) move = false;
        animator.SetBool("Move", move);

    }
    /// <summary>
    /// 攻?アニ?ーシ????。
    /// ・攻?入力を無効化 (canAttack = false)
    /// ・Attack_Combo ステートへ CrossFade
    /// ・手の開閉(rightHand)と攻?タイミ?グを同期
    /// ・コ?ボ段階の更新
    /// ・AttackCD() により?回攻?受付を制御
    /// </summary>
    public override IEnumerator Attack()
    {

        canAttack = false;
        attackTime = gameTimer;
        animator.CrossFade("Attack_Combo", 0.1f);
        animator.SetInteger("Attack_Combo", (int)attackComboType);

        // 攻?判定のタイミ?グ調整（?を開く）
        rightHand.OpenBox();
        yield return WaitForAnimation(AttackAnimeCrip(), 0.8f);
        rightHand.CloseBox();

        // ?のコ?ボ段階を計算
        int nextCombo = (int)attackComboType + 1;

        if (nextCombo >= maxCombo)
            nextCombo = 1;
        attackComboType = (AttackComboType)nextCombo;

        // 攻?クー?ダウ?管?
        if (attackCdCoroutine == null)
            attackCdCoroutine = StartCoroutine(AttackCD());
        yield return attackCdCoroutine;


    }

    /// <summary>
    /// ?ー?（回避）アニ?ーシ????。
    /// ・canRool / canAttack のフ?グ管?
    /// ・"Roll_Start" へ CrossFade
    /// ・"Roll_End" の一定?間経過まで待機
    /// </summary>
    public override IEnumerator Rool()
    {
        rollTime = gameTimer;
        canAttack = true;
        canRool = false;
        animator.CrossFade("Roll_Start", 0.1f);
        yield return new WaitUntil(() =>
                   animator.GetCurrentAnimatorStateInfo(0).IsName("Roll_Start"));
        float loopLimit = animator.GetCurrentAnimatorStateInfo(0).length - 0.1f;
        float loopPart = loopLimit / 10;

        for (float i = 0.1f + loopPart; i < loopLimit; i += loopPart) 
        {
            yield return WaitForAnimation("Roll_Start", i);
            transform.position += transform.forward * 0.15f;
        }


        // Roll_End の 0.5f (50%) まで再生を待機
        yield return WaitForAnimation("Roll_End", 0.5f);

    }


}
