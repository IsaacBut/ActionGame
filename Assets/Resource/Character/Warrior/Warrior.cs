using UnityEngine;

/// <summary>
/// 戦士（Warrior）の実体クラス。  
/// Warrior_Anime を継承し、キャラクターとして必要な初期化処理を実装します。
/// ・アニメーション初期化
/// ・ステータス初期化
/// ・当たり判定（hitBox）の取得
/// ・キャラクターのステージ設定
/// </summary>
public class Warrior : Warrior_Anime
{

    /// <summary>
    /// 戦士の初期化処理。  
    /// ・Collider の取得  
    /// ・アニメーション初期化  
    /// ・ステータス初期化（HP 100 / 攻撃 20 / スタミナ 50）  
    /// ・入力時間とキャラクターステージの初期設定  
    /// </summary>
    public override void CharacterInit()
    {
        // アニメーション初期化
        AnimeInit();
        // ステータス初期化（HP, 攻撃力, スタミナ）
        StatusInit(this.gameObject.GetComponent<BoxCollider>(), 100, 20, 50);
        // 入力時間初期化
        lastInputTime = characterTimer;       
        // キャラクターステージ初期ステート
        nowStage = CharacterStage.Idle;
    }

}
