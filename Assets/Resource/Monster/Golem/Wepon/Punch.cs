using UnityEngine;

public class Punch : Wepon
{
    private void Start()
    {
        WeponInit();
    }

    public override void HitEvent(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit!!!!!!!!!!");
            CloseBox();
            return;
        }
        else return;
    }

}
