using System.Collections;
using UnityEngine;

public class Golem : Golem_Anime
{
    public Area areaOutSide = new Area { radius = 10f };
    public Area areaInSide = new Area { radius = 1f };

    [SerializeField] bool inOutSideArea;
    [SerializeField] bool inInSideArea;

    public Wepon punch;
    public BoxCollider punchBox;

    private Vector3 targetPos = new Vector3(10, 0, 10);

    private void Awake()
    {
        AnimeInit();
        attackCd = 2.5f;
        punchBox.enabled = false;

    }


    private void Update()
    {
        Timer();
        transform.LookAt(targetPos);
        AreaCheck();
        Attack();
        if (!inInSideArea) Move();
        else Stop();
    }

    private void AreaCheck()
    {
        inOutSideArea = areaOutSide.IsAnArea(targetPos, transform.position);
        inInSideArea = areaInSide.IsAnArea(targetPos, transform.position);
    }

    public override void Move()
    {
        base.Move();
        if(IsMoving()) transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 0.5f);

    }

    private void Attack()
    {
        if (!inOutSideArea && !inInSideArea) return;
        //else if (inOutSideArea && !inInSideArea && canAttack) StartCoroutine(ThrowRock());
        //else if (inInSideArea && canAttack) StartCoroutine(Punch());
        else if (inOutSideArea && !inInSideArea && canAttack) StartCoroutine(Punch());
        else if (inInSideArea && canAttack) StartCoroutine(ThrowRock());
    }


    public override IEnumerator Punch()
    {
        punchBox.enabled = true;
        yield return base.Punch();
        punchBox.enabled = false;
        StartCoroutine(CD());
    }

    public override IEnumerator ThrowRock()
    {
        yield return base.ThrowRock();
    }

}
