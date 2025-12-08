using UnityEngine;

public class Sword : Wepon
{

    private void Start()
    {
        WeponInit();
        str = 100;

    }

    public override void HitEvent(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit!!!!!!!!!!");
            CloseBox();
            return;
        }
        else return;
    }



}
