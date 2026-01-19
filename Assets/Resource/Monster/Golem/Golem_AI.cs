using UnityEngine;
using System.Collections;

public class Golem_AI : Monster_Anime
{
    private float _monsterTime => monsterTime;

    private enum AttackType
    {
        Punch = 1, ThrowRock = 2, ComboCount
    }
    [SerializeField] private AttackType attackComboType;

    [Header("Golem")]
    public float throwRockDistance;
    public float puchDistance;
    // ---- 装備情報 ----
    /// <summary>左手に装備している武器（Wepon クラス）</summary>
    public Wepon leftHand;
    /// <summary>右手に装備している武器（Wepon クラス）</summary>
    public ThrowRock rightHand;
    private Vector3 monsterPos => this.transform.position;

    public float areaDistence;
    public override void MonsterInit(BoxCollider box)
    {
        base.MonsterInit(box);
        AnimeInit(this.gameObject.GetComponent<Animator>());
        patrolPoint = new Vector3[4]
        {
            new Vector3(monsterPos.x /areaDistence, monsterPos.y, monsterPos.z * areaDistence),
            new Vector3(monsterPos.x * areaDistence, monsterPos.y, monsterPos.z * areaDistence),
            new Vector3(monsterPos.x * areaDistence, monsterPos.y, monsterPos.z / areaDistence),
            new Vector3(monsterPos.x / areaDistence, monsterPos.y, monsterPos.z / areaDistence),
        };
        patrolPointSet = 0;
    }
    public override void MonsterTimer()
    {
        base.MonsterTimer();
    }

    public override void OnStateExit(MonsterStage stage)
    {
        switch (stage)
        {
            case MonsterStage.Patrol:
                if (patrol != null)
                {
                    StopCoroutine(patrol);
                    patrol = null;
                }
                break;

            case MonsterStage.Seach:
                if (search != null)
                {
                    StopCoroutine(search);
                    search = null;
                }
                break;

            case MonsterStage.LockOn:
                if (lockOn != null)
                {
                    StopCoroutine(lockOn);
                    lockOn = null;
                }
                break;
        }
    }

    private IEnumerator Punch()
    {
        if (attack != null) yield break;
        attackComboType = AttackType.Punch;

        while (true)
        {
            Vector3 dir = lockedPlayer.transform.position - transform.position;
            dir.y = 0f;

            Quaternion targetRot = Quaternion.LookRotation(dir);

            if (Quaternion.Angle(transform.rotation, targetRot) < 1f)
                break;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                720f * Time.deltaTime
            );

            yield return null;
        }


        animator.CrossFade("Punch", 0.1f);
        yield return WaitForAnimation("Punch", 0.4f);
        leftHand.OpenBox();
        yield return WaitForAnimation("Punch", 0.8f);
        leftHand.CloseBox();

    }
    private IEnumerator ThrowRock()
    {
        if (attack != null)
            yield break;

        if (rightHand == null || lockedPlayer == null)
            yield break;

        attackComboType = AttackType.ThrowRock;

        animator.CrossFade("ThrowRock", 0.1f);

        yield return WaitForAnimation("ThrowRock", 0.4f);

        rightHand.CreateRock();

        yield return rightHand.DropSpotAmine(lockedPlayer.transform.position);

        yield return WaitForAnimation("ThrowRock", 0.45f);
        Vector3 dir = lockedPlayer.transform.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720f * Time.deltaTime);
        rightHand.Throw();

        yield return WaitForAnimation("ThrowRock", 0.8f);
        rightHand.NullRock();

    }

    public override IEnumerator Attack()
    {
        if (!canAttack || attack != null)
            yield break;


        canAttack = false;
        attackTime = monsterTime;

        attack = distance > puchDistance
            ? StartCoroutine(ThrowRock())
            : StartCoroutine(Punch());

        yield return attack;

        attack = null;
    }


    private MonsterStage lastStage;

    private void Action()
    {
        if (lastStage != nowStage)
        {
            OnStateExit(lastStage);
            lastStage = nowStage;
        }

        switch (nowStage)
        {
            case MonsterStage.Patrol:
                PatrolAction();
                break;

            case MonsterStage.Seach:
                SeachAction();
                break;
            case MonsterStage.LockOn:
                LockOnAction();
                break;
        }

    }

    private void Awake()
    {
        MonsterInit(this.gameObject.GetComponent<BoxCollider>());
    }

    private void Update()
    {
        MonsterTimer();
        Action();

    }

}
