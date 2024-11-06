using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スコアのUI表示
/// </summary>
public class UIScore : MonoBehaviour {
    private Text scoreText = null;
    private int oldScore = 0;
    
    void Start() {
        scoreText = GetComponent<Text>();
        if (GManager.instance != null) {
            scoreText.text = "Score " + GManager.instance.score;
        }
        else {
            Debug.LogWarning($"[{gameObject.name}] ゲームマネージャーが存在しません！");
            Destroy(this);
        }
    }

    void Update() {
        if (oldScore != GManager.instance.score) {
            scoreText.text = "Score " + GManager.instance.score;
            oldScore = GManager.instance.score;
        }
    }
}
