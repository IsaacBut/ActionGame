using UnityEngine;
using static SkillBuff;

public class Wepon : MonoBehaviour
{
    public AttackElements attackElements;

    public int str { get; set; }
    public float defRedio {  get; set; }

    private BoxCollider boxCollider;

    public void WeponInit()
    {
        attackElements = AttackElements.None;
        boxCollider = this.gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null) Debug.LogWarning("Weapon No Collider");
        else CloseBox();
    }

    public void OpenBox() => boxCollider.enabled = true;
    public void CloseBox() => boxCollider.enabled = false;

    public virtual void HitEvent(Collider other)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        this.HitEvent(other);

    }


}
