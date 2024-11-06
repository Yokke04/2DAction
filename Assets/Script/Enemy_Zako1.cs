using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ザコ敵（仮)
/// </summary>
public class Enemy_Zako1 : MonoBehaviour {
    #region // インスペクターで設定する
    [Header("加算スコア")] public int myScore;
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("画面外でも行動するか")] public bool nonVisibleAct;
    [Header("やられたときのSE")] public AudioClip deadSE;
    //[Header("スプライトオブジェクト")] public GameObject spriteObj;
    [Header("スプライトレンダラー")] public SpriteRenderer sr;
    [Header("アニメーター")] public Animator anim;
    [Header("本体コライダー")] public BoxCollider2D coli;
    [Header("接触判定スクリプト")] public EnemyCollisionTrigger checkCollision;
    #endregion

    #region // プライベート変数
    //private SpriteRenderer sr = null;
    //private Animator anim = null;
    private Rigidbody2D rb = null;
    //private BoxCollider2D coli = null;
    private ObjectStomp oc = null;  // プレイヤーとの衝突の橋渡しスクリプト
    private bool rightTleftF = false;
    private bool isDead = false;
    #endregion

    void Start() {
        bool err = false;

        // コンポーネントのインスタンスを取得
        rb = GetComponent<Rigidbody2D>();
        oc = GetComponent<ObjectStomp>();
        if (rb == null || oc == null) {
            Debug.LogWarning($"[{gameObject.name}] コンポーネントの設定が足りません");
            err = true;
        }

        // 参照設定チェック
        if (deadSE == null || sr == null || anim == null || coli == null || checkCollision == null) {
        //if (checkCollision != null && deadSE != null && spriteObj != null && triggerObj != null) {
        //    sr = spriteObj.GetComponent<SpriteRenderer>();
        //    anim = spriteObj.GetComponent<Animator>();
        //    rb = GetComponent<Rigidbody2D>();
        //    coli = GetComponent<BoxCollider2D>();
        //    oc = GetComponent<ObjectStomp>();
        //}
        //else {
            Debug.LogWarning($"[{gameObject.name}] インスペクターの参照設定が足りません");
            err = true;
        }

        if (err) Destroy(this.gameObject);
    }

    void FixedUpdate() {
        // 通常
        if (!oc.playerStompedOn) {
            if (sr.isVisible || nonVisibleAct) {
                // 進行方向の接触判定で左右反転
                if (checkCollision.isOn) {
                    rightTleftF = !rightTleftF;
                }

                int xVector = -1;
                if (rightTleftF) {
                    xVector = 1;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                rb.velocity = new Vector2(xVector * speed, -gravity);
            }
            // 画面に映っていないとき
            else {
                rb.Sleep();
            }

        }
        // プレイヤーに踏まれた
        else {
            if (!isDead) {
                anim.Play("dead");
                rb.velocity = new Vector2(0, -gravity);
                isDead = true;
                coli.enabled = false;
                if (GManager.instance != null) {
                    GManager.instance.PlaySE(deadSE);
                    GManager.instance.score += myScore;
                }
                Destroy(gameObject, 3f);    // Destroy(オブジェクト, 時間)
            }
            else {
                transform.Rotate(new Vector3(0, 0, 5));
            }
        }
    }
}
