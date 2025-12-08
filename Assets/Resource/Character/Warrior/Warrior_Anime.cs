using UnityEngine;
using System.Collections;

/// <summary>
/// 戦士（Warrior）専用のアニメーション制御クラス。
/// Character_Anime を継承し、移動・停止・攻撃・ロールなど
/// 戦士固有のアニメーション挙動を実装します。
/// </summary>
public class Warrior_Anime : Character_Anime
{
    /// <summary>
    /// アニメーションの初期化処理。
    /// ・Animator の取得
    /// ・バトル／通常状態の初期設定
    /// ・コンボ初期化
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
    /// 移動開始時のアニメーション切り替え処理。
    /// Move フラグが false の場合に true へ変更し、
    /// Animator 側の "Move" パラメータを更新します。
    /// </summary>
    public override void Move()
    {
        if (!move) move = true;
        animator.SetBool("Move", move);
    }
    /// <summary>
    /// 停止時のアニメーション切り替え処理。
    /// Move フラグが true の場合に false へ変更し、
    /// Animator 側の "Move" パラメータを更新します。
    /// </summary>
    public override void Stop()
    {
        if (move) move = false;
        animator.SetBool("Move", move);

    }
    /// <summary>
    /// 攻撃アニメーション処理。
    /// ・攻撃入力を無効化 (canAttack = false)
    /// ・Attack_Combo ステートへ CrossFade
    /// ・手の開閉(rightHand)と攻撃タイミングを同期
    /// ・コンボ段階の更新
    /// ・AttackCD() により次回攻撃受付を制御
    /// </summary>
    public override IEnumerator Attack()
    {

        canAttack = false;
        attackTime = gameTimer;
        animator.CrossFade("Attack_Combo", 0.1f);
        animator.SetInteger("Attack_Combo", (int)attackComboType);

        // 攻撃判定のタイミング調整（箱を開く）
        rightHand.OpenBox();
        yield return WaitForAnimation(AttackAnimeCrip(), 0.8f);
        rightHand.CloseBox();

        // 次のコンボ段階を計算
        int nextCombo = (int)attackComboType + 1;

        if (nextCombo >= maxCombo)
            nextCombo = 1;
        attackComboType = (AttackComboType)nextCombo;

        // 攻撃クールダウン管理
        if (attackCdCoroutine == null)
            attackCdCoroutine = StartCoroutine(AttackCD());
        yield return attackCdCoroutine;


    }

    /// <summary>
    /// ロール（回避）アニメーション処理。
    /// ・canRool / canAttack のフラグ管理
    /// ・"Roll_Start" へ CrossFade
    /// ・"Roll_End" の一定時間経過まで待機
    /// </summary>
    public override IEnumerator Rool()
    {
        rollTime = gameTimer;
        canAttack = true;
        canRool = false;
        animator.CrossFade("Roll_Start", 0.1f);

        // Roll_End の 0.5f (50%) まで再生を待機
        yield return WaitForAnimation("Roll_End", 0.5f);

    }


}
