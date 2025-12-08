using System.Drawing;
using UnityEngine;

public class Area
{
    public float radius{  get;  set; }
    private float radiusSqr => radius * radius;

    public bool IsAnArea(Vector3 target, Vector3 self) => (target - self).sqrMagnitude <= radiusSqr;

}
