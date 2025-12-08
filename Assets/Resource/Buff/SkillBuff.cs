using UnityEngine;

/// <summary>
/// スキルに付与される追加効果（属性など）を管理するバフデータ。
/// 今は攻撃属性の付与・変更に使用される。
/// </summary>
public class SkillBuff
{
    /// <summary>
    /// 攻撃属性の種類。
    /// None = 無属性
    /// Fire = 火属性
    /// Ice  = 氷属性
    /// </summary>
    [SerializeField]
    public enum AttackElements
    {
        None,
        Fire,
        Ice
    }
    /// <summary>
    /// 現在付与されている攻撃属性
    /// </summary>
    public AttackElements attackElements {  get; set; }

}
