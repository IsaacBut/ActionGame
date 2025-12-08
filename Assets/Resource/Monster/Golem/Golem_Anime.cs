using UnityEngine;
using static Warrior_Anime;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class Golem_Anime : Monster
{
    private enum AttackType
    {
        Punch = 1, ThrowRock = 2, ComboCount
    }
    [SerializeField] private AttackType attackComboType;

    private bool move;
    public bool IsMoving() => animator.GetCurrentAnimatorStateInfo(0).IsName("Move");
    public bool canAttack;

    private float gameTimer;
    private float attackTime;
    public float attackCd {  get; set; }

    public void Timer()
    {
        gameTimer += Time.deltaTime;
    }



    public Animator animator { get; private set; }

    public void AnimeInit()
    {
        animator = this.gameObject.GetComponent<Animator>();
    }

    public virtual void Move()
    {
        if (!move) move = true;
        animator.SetBool("Move", move);
    }

    public void Stop()
    {
        if (move) move = false;
        animator.SetBool("Move", move);

    }

    private Coroutine attackCdCoroutine;

    private IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(attackCd);
        canAttack = true;
        attackCdCoroutine = null;
    }

    public IEnumerator CD()
    {
                if (attackCdCoroutine == null)
            attackCdCoroutine = StartCoroutine(AttackCD());
        yield return attackCdCoroutine;
    }

    public IEnumerator WaitForAnimation(string stateName, float time)
    {
        // 確保 animator 已進入該狀態
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        // 等動畫播放完 (normalizedTime >= 1)
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= time);

        Debug.Log(stateName + "Animation Finished!");
    }


    public virtual IEnumerator Punch()
    {
        canAttack = false;
        animator.CrossFade("Punch", 0.1f);
        yield return WaitForAnimation("Punch", 0.8f);

    }

    public virtual IEnumerator ThrowRock()
    {
        canAttack = false;
        animator.CrossFade("ThrowRock", 0.1f);
        yield return WaitForAnimation("ThrowRock", 0.8f);
        if (attackCdCoroutine == null)
            attackCdCoroutine = StartCoroutine(AttackCD());
        yield return attackCdCoroutine;
    }

}
