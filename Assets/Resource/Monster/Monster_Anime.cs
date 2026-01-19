using UnityEngine;
using System.Collections;

public class Monster_Anime : Monster
{
    public Animator animator { get;  private set; }
    private float _monsterTimer => monsterTime;

    public virtual void AnimeInit(Animator targetAnimator) 
    {
        animator = targetAnimator;
    }
    public IEnumerator WaitForAnimation(string stateName, float time)
    {
        // 
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        // ?
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= time);

        //Debug.Log(stateName + "Animation Finished!");
    }

    public bool move;
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


    public Coroutine attack;
    public Coroutine lockOn;
    public Coroutine patrol;
    public Coroutine search;

    public int patrolPointSet;

    public void PatrolAction()
    {
        if (patrol == null)
        {
            patrol = StartCoroutine(Patrol());
        }
    }


    public virtual IEnumerator Patrol()
    {
        while (nowStage == MonsterStage.Patrol)
        {
            int nextIndex = (patrolPointSet + 1) % patrolPoint.Length;
            Vector3 targetPoint = patrolPoint[nextIndex];
            Vector3 dir = targetPoint - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(dir);

            dir.y = 0f;

            while ((transform.position - targetPoint).sqrMagnitude > 0.01f)
            {
                if (transform.rotation != targetRot) yield return RotateTo(targetRot, 1);
                Move();
                transform.rotation = Quaternion.LookRotation(dir);
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPoint,
                    patrolSpeed * Time.deltaTime
                );
                if (distance <= patrolSeachingDistance)
                {
                    nowStage = MonsterStage.Seach;
                    yield return null;
                }


                yield return null;
            }

            if ((transform.position - targetPoint).sqrMagnitude <= 0.01f)
            {
                transform.position = targetPoint;
                patrolPointSet = nextIndex;

                nowStage = MonsterStage.Seach;
            }
        }
    }

    float searchingTime;
    float turningTime = 1f;
    public void SeachAction()
    {
        if (search == null) search = StartCoroutine(Seach());
    }

    public virtual IEnumerator Seach()
    {
        Stop();

        float timer = 0f;
        float baseY = transform.eulerAngles.y;

        Quaternion rightRot = Quaternion.Euler(0, baseY + 45f, 0);
        Quaternion leftRot = Quaternion.Euler(0, baseY - 45f, 0);

        yield return RotateTo(rightRot, turningTime);

        while (timer < seachingTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return RotateTo(leftRot, turningTime);

        if (distance <= followLostDistance)
        {
            while (true)
            {
                Vector3 dir = lockedPlayer.transform.position - transform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude < 0.001f)
                    break;

                Quaternion targetRot = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    5f * Time.deltaTime
                );

                // ⭐ 判断角度是否足够接近
                if (Quaternion.Angle(transform.rotation, targetRot) < 1f)
                    break;

                yield return null;
            }
            nowStage = MonsterStage.LockOn;

        }
        else nowStage = MonsterStage.Patrol;
        yield break;
    }
    public virtual IEnumerator Attack() {  yield return 0; }


    float lockOnTime;
    public void LockOnAction()
    {
        if (!locked && distance > followLostDistance) return;
        locked = true;
        if (distance > followLostDistance)
        {
            lockOnTime += Time.deltaTime;
            if (lockOnTime > followLostTime)
            {
                locked = false;
                if (lockOn != null)
                {
                    StopCoroutine(lockOn);
                    lockOn = null;
                }
                nowStage = MonsterStage.Seach;
                return;
            }
        }
        else
        {
            lockOnTime = 0f; // ⭐重新锁定就清零
        }

        if (canAttack)
        {
            if (lockOn != null)
            {
                StopCoroutine(lockOn);
                lockOn = null;
            }

            StartCoroutine(Attack());
            return;
        }
        else if (!canAttack && lockOn == null)
        {
            lockOn = StartCoroutine(LockOn());
        }

    }

    public virtual IEnumerator LockOn()
    {
        while (nowStage == MonsterStage.LockOn)
        {
            if (!lockedPlayer) yield break;

            if (attack != null)
            {
                Stop();
                yield return null;
                continue;
            }
            if (distance < stopMove)
            {
                Stop();
                break;
            }
            Vector3 dir = lockedPlayer.transform.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    rot,
                    42 * Time.deltaTime
                );
            }


            Move();
            transform.position = Vector3.MoveTowards(
                transform.position,
                lockedPlayer.transform.position,
                lockedMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        lockOn = null;
    }
    public virtual IEnumerator RotateTo(Quaternion target, float rotateTime)
    {
        Quaternion start = transform.rotation;
        float timer = 0f;
        if(Quaternion.Angle(start, target) < 0.01f) yield break;

        while (timer < rotateTime)
        {
            timer += Time.deltaTime;
            float t = timer / rotateTime;

            transform.rotation = Quaternion.Lerp(start, target, t);
            yield return null;
        }

        transform.rotation = target;
    }


}
