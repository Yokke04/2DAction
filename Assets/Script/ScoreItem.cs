using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコアアイテム
/// </summary>
public class ScoreItem : MonoBehaviour {
    [Header("加算するスコア")] public int myScore;
    [Header("プレイヤーの判定")] public PlayerTriggerCheck playerCheck;
    [Header("アイテム取得時SE")] public AudioClip itemSE;

    void Update() {
        if (playerCheck.isOn) {
            if (GManager.instance != null) {
                GManager.instance.score += myScore;
                GManager.instance.PlaySE(itemSE);
                Destroy(this.gameObject);
            }
        }
    }
}
