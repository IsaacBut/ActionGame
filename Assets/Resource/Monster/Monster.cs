using UnityEngine;
using static Character;

public class Monster : MonoBehaviour
{
    public BoxCollider hitBox { get; private set; }

    [Header("Monster Basic")]

    public int hp;
    public int str;
    public int def;


    private float redio;

    [Header("Stage___Patrol")]
    public float patrolSeachingDistance;
    public float patrolSpeed;
    public Vector3[] patrolPoint;

    [Header("Stage___Seaching")]
    public float seachingDistance;
    //public float seachingCoolDown;
    public float seachingTime = 2.0f;

    [Header("Stage___LockOn")]
    public GameObject lockedPlayer;
    public float distance => Mathf.Abs(Vector3.Distance(transform.position, lockedPlayer.transform.position));

    public float stopMove;
    public float followLostDistance;
    public float followLostTime = 0.5f;

    public bool locked;
    public float lockedMoveSpeed => patrolSpeed * 1.5f;

    public float attackCoolDown;
    public float attackTime = 3.0f;
    public bool canAttack;

    public float monsterTime { get; private set; }
    private float Timer() => monsterTime += Time.deltaTime;
    public GameObject player { get; private set; }

    public enum MonsterStage
    {
        Patrol = 0,
        Seach,
        LockOn,
        Dead
    }
    public MonsterStage nowStage;

    public virtual void OnStateExit(MonsterStage stage)
    {
       
    }


    public virtual void MonsterInit(BoxCollider box)
    {
        player = GameObject.FindWithTag("Player");
        hitBox = box;
        redio = hitBox.bounds.size.z;

        patrolSeachingDistance *= redio;
        seachingDistance *= redio;
        followLostDistance *= redio;

    }

    public virtual void MonsterTimer()
    {
        Timer();
        canAttack = monsterTime >= attackTime + attackCoolDown;
    }

    public bool IsDead()
    {
        if (hp <= 0)
        {
            nowStage = MonsterStage.Dead;
            return true;
        }
        return false;
    }

    public float PlayerDistance() => Vector3.Distance(this.transform.position, player.transform.position);

}
