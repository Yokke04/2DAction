using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落ちる床の制御
/// </summary>
public class FallDownFloor : MonoBehaviour {
    [Header("振動幅")] public float vibrationWidth = 0.05f;
    [Header("振動速度")] public float vibrationSpeed = 30.0f;
    [Header("落ちるまでの時間")] public float fallTime = 1.0f;
    [Header("落ちる速度")] public float fallSpeed = 10.0f;
    [Header("落ちてから戻ってくる時間")] public float returnTime = 5.0f;
    [Header("振動アニメーション")] public AnimationCurve curve;
    [Header("本体オブジェクト")] public GameObject baseObj;
    [Header("スプライトレンダラー")] public SpriteRenderer sr;

    private bool isOn;
    private bool isFall;
    private bool isReturn;
    private Vector3 spriteDefaultPos;
    private Vector3 floorDefaultPos;
    private Vector2 fallVelocity;
    private Transform tf = null;
    private Rigidbody2D rb = null;
    private ObjectStomp oc = null;
    private float timer = 0.0f;
    private float fallingTimer = 0.0f;
    private float returnTimer = 0.0f;
    private float blinkTimer = 0.0f;
    
    void Start() {
        bool err = false;

        oc = GetComponent<ObjectStomp>();
        if (sr != null && oc != null && baseObj != null) {
            tf = baseObj.transform;
            rb = baseObj.GetComponent<Rigidbody2D>();
            if (rb != null) {
                spriteDefaultPos = sr.transform.position;
                fallVelocity = new Vector2(0, -fallSpeed);
                floorDefaultPos = tf.position;
            }
            else {
                Debug.LogWarning($"[{GManager.GetHierarchyPath(baseObj, gameObject)}] リジッドボディが設定されていません");
                err = true;
            }
        }
        else {
            Debug.LogWarning($"[{gameObject.name}] インスペクターの参照設定に不備があります");
            err = true;
        }

        if (err) Destroy(this.gameObject);
    }

    void Update() {
        // プレイヤーが1回でも乗ったらフラグをON
        if (oc.playerStompedOn) {
            isOn = true;
            oc.playerStompedOn = false;
        }

        // 振動する
        if (isOn && !isFall) {
            float x = curve.Evaluate(timer * vibrationSpeed) * vibrationWidth;
            //float x = vibrationWidth * Mathf.Sin(vibrationSpeed * timer);   // 三角関数で揺らす場合
            sr.transform.position = spriteDefaultPos + new Vector3(x, 0, 0);

            // 一定時間たったら落ちる
            if (timer > fallTime) {
                isFall = true;
            }

            timer += Time.deltaTime;
        }

        // 一定時間たつと明滅して戻ってくる
        if (isReturn) {
            // 明滅　ついている時に戻る
            if (blinkTimer > 0.2f) {
                SwitchRendererVisible(sr, true);
                blinkTimer = 0.0f;
            }
            // 明滅　消えている時
            else if (blinkTimer > 0.1f) {
                SwitchRendererVisible(sr, false);
            }
            // 明滅　ついている時
            else {
                SwitchRendererVisible(sr, true);
            }

            // 1秒たったら明滅終わり
            if (returnTimer > 1.0f) {
                isReturn = false;
                blinkTimer = 0f;
                returnTimer = 0f;
                //sr.enabled = true;
                SwitchRendererVisible(sr, true);
            }
            else {
                blinkTimer += Time.deltaTime;
                returnTimer += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate() {
        // 落下中
        if (isFall) {
            rb.velocity = fallVelocity;

            // 一定時間たつと元の位置に戻る
            if (fallingTimer > returnTime) {
                isReturn = true;
                tf.position = floorDefaultPos;
                rb.velocity = Vector2.zero;
                isFall = false;
                timer = 0.0f;
                fallingTimer = 0.0f;
            }
            else {
                fallingTimer += Time.deltaTime;
                isOn = false;
            }
        }
    }

    /// <summary>
    /// スプライトレンダラーの表示を親子まとめて切り替える
    /// </summary>
    /// <param name="parentSr">親レンダラー</param>
    /// <param name="enabled"></param>
    private void SwitchRendererVisible(SpriteRenderer parentSr, bool enabled) {
        SpriteRenderer[] childSrs = parentSr.GetComponentsInChildren<SpriteRenderer>();

        parentSr.enabled = enabled;
        foreach (var sr in childSrs) {
            sr.enabled = enabled;
        }
    }
}
