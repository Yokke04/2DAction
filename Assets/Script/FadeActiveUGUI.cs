using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メッセージのフェード制御
/// </summary>
public class FadeActiveUGUI : MonoBehaviour {
    [Header("フェードスピード")] public float speed = 1.0f;
    [Header("上昇量")] public float moveDis = 10.0f;
    [Header("上昇時間")] public float moveTime = 1.0f;
    [Header("キャンバスグループ")] public CanvasGroup cg;
    [Header("プレイヤー判定")] public PlayerTriggerCheck trigger;
    
    private Vector3 defaultPos;
    private float timer = 0.0f;

    void Start() {
        // 初期化
        if (cg == null && trigger == null) {
            Debug.LogWarning($"[{gameObject.name}] インスペクターの設定が足りません");
            Destroy(this);
        }
        else {
            cg.alpha = 0.0f;
            defaultPos = cg.transform.position;
            cg.transform.position = defaultPos - Vector3.up * moveDis;  // 下からフェードインするために少し下にずらす
        }
    }

    void Update() {
        // プレイヤーが選択範囲に入った
        if (trigger.isOn) {
            // 上昇しながらフェードイン
            if (cg.transform.position.y < defaultPos.y || cg.alpha < 1.0f) {
                cg.alpha = timer / moveTime;
                cg.transform.position += Vector3.up * (moveDis / moveTime) * speed * Time.deltaTime;
                timer += speed * Time.deltaTime;
            }
            // フェードイン完了
            else {
                cg.alpha = 1.0f;
                cg.transform.position = defaultPos;
            }
        }
        // プレイヤーが範囲内にいない
        else {
            // 下降しながらフェードアウト
            if (cg.transform.position.y > defaultPos.y - moveDis || cg.alpha > 0.0f) {
                cg.alpha = timer / moveTime;
                cg.transform.position -= Vector3.up * (moveDis / moveTime * speed * Time.deltaTime);
                timer -= speed * Time.deltaTime;
            }
            // フェードアウト完了
            else {
                timer = 0.0f;
                cg.alpha = 0.0f;
                cg.transform.position = defaultPos -Vector3.up * moveDis;
            }
        }
    }
}
