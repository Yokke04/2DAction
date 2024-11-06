using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 残りプレイヤー数のUI表示
/// </summary>
public class UIRemainingPlayers : MonoBehaviour {
    private Text numberText = null;
    private int preRemPlayers = 0;

    void Start() {
        numberText = GetComponent<Text>();
        if (GManager.instance != null) {
            numberText.text = "× " + GManager.instance.remPlayers;
        }
        else {
            Debug.LogWarning($"[{gameObject.name}] ゲームマネージャーが存在しません！");
            Destroy(this);
        }
    }

    void Update() {
        if (preRemPlayers != GManager.instance.remPlayers) {
            numberText.text = "× " + GManager.instance.remPlayers;
            preRemPlayers = GManager.instance.remPlayers;
        }
    }
}
