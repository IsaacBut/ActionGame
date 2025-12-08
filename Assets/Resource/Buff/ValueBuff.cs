using UnityEngine;

/// <summary>
/// ステータス値（HP / MP / STR）を上昇させるバフを管理するクラス。
/// キャラクターの基本能力値に対して加算・補正を行う際に使用する。
/// </summary>
public class ValueBuff
{
    /// <summary>HPの上昇値</summary>
    public float hp { get; set; }
    /// <summary>MPの上昇値</summary>
    public float mp { get; set; }
    /// <summary>STR（攻撃力など）の上昇値</summary>
    public float str { get; set; }
}
