using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ステージ番号のUI表示
/// </summary>
public class UIStageNum : MonoBehaviour {
    private Text stageText = null;
    private int oldStageNum = 0;

    void Start() {
        stageText = GetComponent<Text>();
        if (GManager.instance != null) {
            stageText.text = "Score " + GManager.instance.stageNum;
        }
        else {
            Debug.LogWarning($"[{gameObject.name}] ゲームマネージャーが存在しません！");
            Destroy(this);
        }
    }
    
    void Update() {
        if (oldStageNum != GManager.instance.stageNum) {
            stageText.text = "Stage " + GManager.instance.stageNum;
            oldStageNum = GManager.instance.stageNum;
        }
    }
}
