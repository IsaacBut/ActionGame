using UnityEngine;
using static SkillBuff;

public class Wepon : MonoBehaviour
{
    public AttackElements attackElements;

    public int str { get; set; }
    public float defRedio {  get; set; }

    public new Collider collider;

    public void WeponInit()
    {
        attackElements = AttackElements.None;
        collider = this.gameObject.GetComponent<Collider>();
        if (collider == null) Debug.LogWarning("Weapon No Collider");
        else CloseBox();
    }

    public void OpenBox() => collider.enabled = true;
    public void CloseBox() => collider.enabled = false;

    public virtual void HitEvent(Collider other)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        this.HitEvent(other);

    }


}
