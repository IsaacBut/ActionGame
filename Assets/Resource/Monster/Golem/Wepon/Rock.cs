using UnityEngine;

public class Rock : Wepon
{
    public Vector3 targetPos = new Vector3(99999, 99999, 9999);
    private bool locked => Vector3.Distance(targetPos, new Vector3(99999, 99999, 9999)) > 0.1f;
    private float duration;
    private float height;

    private float timer;

    public void Launch(float time, float arcHeight)
    {
        duration = time;
        height = arcHeight;
        timer = 0f;
        OpenBox();
    }
    private void Start()
    {
        WeponInit();
        str = 100;
        Launch(10f, 0.5f);
    }

    private void Update()
    {
        if (locked) Action();
    }
    //public void Action()
    //{
    //    timer += Time.deltaTime;
    //    float t = Mathf.Clamp01(timer / duration);

    //    // 水平插值
    //    Vector3 pos = Vector3.Lerp(transform.position, targetPos, t);

    //    // 垂直拱起（拋物线）
    //    float yOffset = height * 4f * t * (1f - t);
    //    pos.y += yOffset;

    //    transform.position = pos;

    //    // 落地
    //    if (t >= 1f)
    //    {
    //        OnArrive();
    //    }
    //}

    public void Action()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        transform.position = Vector3.Lerp(transform.position, targetPos, t);

    }

    public override void HitEvent(Collider other)
    {
        if (other != null) Debug.Log(other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit!!!!!!!!!!");
            CloseBox();
            Destroy(gameObject);
            return;
        }
        else if (other.CompareTag("Building")) Destroy(gameObject);
        else return;
    }
}
