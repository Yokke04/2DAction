using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動する床の制御
/// </summary>
public class MoveObject : MonoBehaviour, MoveObjectIF {
    [Header("移動経路")] public GameObject[] movePoint;
    [Header("速さ")] public float speed = 1.0f;
    [Header("本体オブジェクト")] public GameObject baseObj;

    private Transform tf = null;
    private Rigidbody2D rb = null;
    private int nowPoint = 0;
    private bool returnPoint = false;
    // 乗った物体の慣性キャンセル用
    private Vector2 oldPos = Vector2.zero;
    private Vector2 mvVelocity = Vector2.zero;
    
    void Start() {
        bool err = false;

        if (movePoint != null && movePoint.Length > 0 && baseObj != null) {
            tf = baseObj.transform;
            rb = baseObj.GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.position = movePoint[0].transform.position;
                oldPos = rb.position;
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

    /// <summary>
    /// 動いた距離を返す
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVelocity() {
        return mvVelocity;
    }

    void FixedUpdate() {
        // 通常進行
        if (!returnPoint) {
            int nextPoint = nowPoint + 1;
            // 目標のポイントとの誤差がわずかになるまで移動
            if (Vector2.Distance(tf.position, movePoint[nextPoint].transform.position) > 0.1f) { // Vector2.Distance：2つの地点の距離を返す
                // 現在地から次のポイントへのベクトルを作成
                Vector2 toVector = Vector2.MoveTowards(
                    tf.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);   // Vector2.MoveTowards(A地点, B地点, 最大距離)：A地点からB地点へのベクトル上の座標を返す
                // 次のポイントへの移動
                rb.MovePosition(toVector);  // Rigidbody2D.MovePosition(位置)：物体を指定した位置へ移動する(※物理計算が行われる。コライダーがついたものをtransformで動かすと再計算が発生し無駄に重くなるのでこれを使用する)
            }
            // 目標のポイントに到達
            else {
                rb.MovePosition(movePoint[nextPoint].transform.position);
                ++nowPoint;

                // 移動経路配列の最後だった場合
                if (nowPoint + 1 >= movePoint.Length) {
                    returnPoint = true;
                }
            }

        }
        // 逆進行
        else {
            int nextPoint = nowPoint - 1;
            // 目標のポイントとの誤差がわずかになるまで移動
            if (Vector2.Distance(tf.position, movePoint[nextPoint].transform.position) > 0.1f) {
                // 現在地から次のポイントへのベクトルを作成
                Vector2 toVector = Vector2.MoveTowards(
                    tf.position, movePoint[nextPoint].transform.position, speed * Time.deltaTime);
                // 次のポイントへの移動
                rb.MovePosition(toVector);
            }
            // 目標のポイントに到達
            else {
                rb.MovePosition(movePoint[nextPoint].transform.position);
                --nowPoint;

                // 移動経路配列の最初だった場合
                if (nowPoint <= 0) {
                    returnPoint = false;
                }
            }

        }
        // 動いた距離を格納
        mvVelocity = (rb.position - oldPos) / Time.deltaTime;
        oldPos = rb.position;
    }
}
