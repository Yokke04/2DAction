using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー制御
/// </summary>
public class Player : MonoBehaviour {
    #region // インスペクターで設定する変数
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ジャンプする高さ")] public float jumpHeight;
    [Header("ジャンプ時間")] public float jumpLimitTime;
    [Header("踏みつけ判定の高さの割合(%)")] public float stepOnRate;
    [Header("ダッシュの加速曲線")] public AnimationCurve dashCurve;
    [Header("ジャンプの加速曲線")] public AnimationCurve jumpCurve;
    [Header("ジャンプSE")] public AudioClip jumpSE;
    [Header("ミス時のSE")] public AudioClip downSE;
    [Header("落下時のSE")] public AudioClip fallSE;
    [Header("リスポーン時のSE")] public AudioClip respawnSE;
    [Header("スプライトレンダラー")] public SpriteRenderer sr;
    [Header("アニメーター")] public Animator anim;
    [Header("アニメーション制御スクリプト")] public PlayerAnimation animCtrl;
    //[Header("リジッドボディ")] public Rigidbody2D rb;
    [Header("本体コライダー")] public CapsuleCollider2D coli;
    [Header("接地判定スクリプト")] public GroundCheck ground;
    [Header("天井判定スクリプト")] public GroundCheck head;
    #endregion

    #region // プライベート変数
    // コンポーネント
    //private SpriteRenderer sr = null;
    //private Animator anim = null;
    private Rigidbody2D rb = null;
    //private CapsuleCollider2D coli = null;
    private MoveObjectIFCaller moveObj = null;
    // InputSystem
    private Vector2 moveInput;
    private bool jumpInput;
    // フラグ
    private bool isRun = false;     // 左右移動中
    private bool isJump = false;    // ジャンプ中
    private bool isJumpKeyPress = false;    // ジャンプキー押下開始(押しっぱなし防止)
    private bool isJumpKeyRelease = false;  // ジャンプキー押下後に離された(押しっぱなし防止)
    private bool isBounce = false;  // 跳ねている最中
    private bool isLanding = false; // 着地モーション中
    private bool isHead = false;    // 天井に接地
    private bool isGround = false;  // 地面に接地
    private bool isDown = false;    // ミスアニメーション中
    private bool isRespawn = false; // リスポーンアニメーション中
    private bool isClearMotion = false; // クリアアニメーション中
    private bool nonDownAnim = false;   // ミスアニメーションをしない(落下など)
    // パラメータ
    private float jumpPos = 0.0f;   // ジャンプ開始時の高さ
    private float otherJumpHeight = 0.0f;  // 跳ねた時の高さ
    private float dashTime = 0.0f;  // ダッシュ時間
    private float jumpTime = 0.0f;  // ジャンプ可能時間
    private float beforeKey = 0.0f; // 前の入力
    private float respawnTime = 0.0f;   // リスポーンアニメーション時間
    private float blinkTime = 0.0f; // リスポーン時の点滅時間
    // アニメーション
    private string animIdle = "player_idle";
    private string animWin = "player_win";
    private string animDown = "player_down";
    // タグ
    private string enemyTag = "Enemy";
    private string deadAreaTag = "DeadArea";
    private string hitAreaTag = "HitArea";
    private string moveFloorTag = "MoveFloor";
    private string fallFloorTag = "FallFloor";
    #endregion

    void Start() {
        bool err = false;

        // コンポーネントのインスタンスを取得
        //sr = GetComponentInChildren<SpriteRenderer>();
        //anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //coli = GetComponent<CapsuleCollider2D>();
        if (rb == null) {
            Debug.LogWarning($"[{gameObject.name}] リジッドボディが設定されていません");
            err = true;
        }

        // 参照設定チェック
        if (jumpSE == null || downSE == null || fallSE == null || respawnSE == null || 
            sr == null || anim == null || animCtrl == null || 
            coli == null || ground == null || head == null) {
            //sr == null || anim == null || rb == null || coli == null || ground == null || head == null) {
            Debug.LogWarning($"[{gameObject.name}] インスペクターの参照設定が足りません");
            err = true;
        }

        if (err) Destroy(this.gameObject);
    }

    void Update() {
        respawnAnimation();
    }

    void FixedUpdate() {
        // 操作可能時
        if (!isDown && !GManager.instance.isGameOver && !GManager.instance.isStageClear) {
            // 接地判定を取得する
            isGround = ground.IsGround();
            isHead = head.IsGround();

            // キーを入力されたら行動する
            float xSpeed = GetXSpeed();
            float ySpeed = GetYSpeed();

            // アニメーションを適用
            SetAnimation();

            // 移動速度を適用
            Vector2 addVelocity = Vector2.zero;
            // 動く床に乗っているときに慣性をキャンセルする
            if (moveObj != null) {
                addVelocity = moveObj.GetVelocity();
            }

            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;   // 重力にインスペクター指定値を代入
        }
        // 操作不可の時
        else {
            // ゲームマネージャーにクリア通知が渡っていればクリア演出開始
            if (!isClearMotion && GManager.instance.isStageClear) {
                anim.Play(animWin);
                isClearMotion = true;
            }
            rb.velocity = new Vector2(0, -gravity);
        }
    }

    /// <summary>
    /// Moveアクションのコールバック(InputSystem)
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// X成分で必要な計算をし、速度を返す
    /// </summary>
    /// <returns>X軸の速さ</returns>
    private float GetXSpeed() {
        //float horizontalKey = Input.GetAxis("Horizontal");  // InputManager
        float horizontalKey = moveInput.x;  // InputSystem
        float xSpeed = 0.0f;

        // 平行移動
        if (horizontalKey > 0) {
            transform.localScale = new Vector3(1, 1, 1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        else if (horizontalKey < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        else {
            isRun = false;
            xSpeed = 0.0f;
            dashTime = 0.0f;
        }

        // 前回の入力からダッシュの反転を判断して速度を変える（加速リセット）
        if (horizontalKey > 0 && beforeKey < 0) {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0) {
            dashTime = 0.0f;
        }
        // アニメーションカーブを速度に適用
        xSpeed *= dashCurve.Evaluate(dashTime); // AnimationCurve.Evaluate(時間) カーブから引数時間の値を返す
        beforeKey = horizontalKey;

        return xSpeed;
    }

    /// <summary>
    /// Jumpアクションのコールバック(InputSystem)
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context) {
        if (context.performed) {
            jumpInput = true;
            //if (!isJumpKeyPress) Debug.Log($"[OnJump] ジャンプキー受付開始 Press[{true}] Release[{isJumpKeyRelease}]");
            isJumpKeyPress = true;
        }
        else if (context.canceled) {
            jumpInput = false;
            if (isJumpKeyPress) {
                isJumpKeyRelease = true;
                //Debug.Log($"[OnJump] ジャンプキー受付終了 Press[{isJumpKeyPress}] Release[{isJumpKeyRelease}]");
            }
        }
    }

    /// <summary>
    /// Y成分で必要な計算をし、速度を返す
    /// </summary>
    /// <returns>Y軸の速さ</returns>
    private float GetYSpeed() {
        //float verticalKey = Input.GetAxis("Jump");  // InputManager
        float ySpeed = -gravity;

        // オブジェクトを踏んで跳ねた時
        if (isBounce) {
            bool canHeight = jumpPos + otherJumpHeight > transform.position.y;  // 現在の高さがジャンプ限界高度より低いか
            bool canTime = jumpLimitTime > jumpTime;    // ジャンプ可能時間を超えていないか

            // ジャンプ入力受け付け条件判定(上記3つ＋頭ぶつけてないか)
            if (canHeight && canTime && !isHead) {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else {
                isBounce = false;
                jumpTime = 0.0f;
            }
        }
        // ジャンプ中
        else if (isJump) {
            //bool pushUpKey = verticalKey > 0;   // ジャンプキーを押しているか
            bool canHeight = jumpPos + jumpHeight > transform.position.y;   // 現在の高さがジャンプ限界高度より低いか
            bool canTime = jumpLimitTime > jumpTime;    // ジャンプ可能時間を超えていないか

            // ジャンプ入力受け付け条件判定(上記3つ＋頭ぶつけてないか＋着地モーション中じゃないか)
            //if (pushUpKey && canHeight && canTime && !isHead) {
            if (jumpInput && !isLanding && canHeight && canTime && !isHead) {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            // ジャンプ入力限界
            else {
                isJump = false;
                jumpTime = 0.0f;
                //if (!isJumpKeyRelease) Debug.Log($"[OnJump] ジャンプ限界でキー受付終了 Press[{isJumpKeyPress}] Release[{true}]");
                isJumpKeyRelease = true;
            }
        }
        // 地面にいる
        else if (isGround) {
            // ジャンプする
            //if (verticalKey > 0) {
            if (jumpInput && !isJumpKeyRelease && !isLanding) {    // InputSystem
                if (!isJump) {   // 重複再生防止
                    GManager.instance.PlaySE(jumpSE);
                }
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; // ジャンプ開始時の高さを保存
                isJump = true;
                jumpTime = 0.0f;    // ジャンプ滞空時間をリセット
            }
            // ジャンプしない
            else {
                isJump = false;
                // 押しっぱなし防止フラグリセット(地面にいる+キー入力なし+着地の入力不可フレームではない)
                //if (!jumpInput && isJumpKeyRelease && !isLanding) {
                if (!jumpInput && isJumpKeyRelease) {
                    isJumpKeyPress = false;
                    isJumpKeyRelease = false;
                    //Debug.Log($"[LandComp] 押しっぱなし防止フラグリセット Press[{isJumpKeyPress}] Release[{isJumpKeyRelease}]");
                }
            }
        }

        // アニメーションカーブを速度に適用
        if (isJump || isBounce) {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }

        return ySpeed;
    }

    /// <summary>
    /// アニメーターのパラメーターを変更する
    /// </summary>
    private void SetAnimation() {
        anim.SetBool("jump", isJump || isBounce);
        anim.SetBool("ground", isGround);
        anim.SetBool("run", isRun);

    }

    #region // ***** 着地イベントを受け取る *****
    // *** イベントリスナー管理 ***
    // スクリプトが有効になったときに追加
    void OnEnable() {
        //ground.OnTriggerEnterEvent += OnTriggerEnterReceived;
        animCtrl.OnLandingStart += OnLandingStartReceived;
        animCtrl.OnLandingComplete += OnLandingCompleteReceived;
    }

    // スクリプトが無効になったときに削除
    void OnDisable() {
        //ground.OnTriggerEnterEvent -= OnTriggerEnterReceived;
        animCtrl.OnLandingStart -= OnLandingStartReceived;
        animCtrl.OnLandingComplete -= OnLandingCompleteReceived;
    }

    // *** イベント受け取り ***
    /// <summary>
    /// 接地トリガー発火通知受け取り
    /// </summary>
    /*void OnTriggerEnterReceived(Collider2D collider) {
        if (isJump) {
             isLanding = true;
        }
        //Debug.Log($"接地トリガー発火 isLanding:{isLanding}");
    }*/

    /// <summary>
    /// 着地アニメーション開始
    /// </summary>
    void OnLandingStartReceived() {
        isLanding = true;
        //Debug.Log($"着地アニメーション開始 isLanding:{isLanding}");
    }

    /// <summary>
    /// 着地アニメーション完了
    /// </summary>
    void OnLandingCompleteReceived() {
        isLanding = false;
        //Debug.Log($"着地アニメーション完了 isLanding:{isLanding}");
    }
    #endregion

    #region // 接触判定
    private void OnCollisionEnter2D(Collision2D collision) {
        // フラグ整理
        bool enemy = (collision.collider.tag == enemyTag);
        bool moveFloor = (collision.collider.tag == moveFloorTag);
        bool fallFloor = (collision.collider.tag == fallFloorTag);

        if (enemy || moveFloor || fallFloor) {
            // 踏みつけ判定になる高さ(プレイヤーのコライダー高さのstepOnRate％)
            float stepOnHeight = (coli.size.y * (stepOnRate / 100f));
            // 踏みつけ判定のワールド座標
            float judgePos = coli.transform.position.y - (coli.size.y / 2f) + stepOnHeight;

            foreach (ContactPoint2D cp in collision.contacts) {
                // 衝突位置が自分の中心位置より下だったら
                if (cp.point.y < judgePos) {
                    if (enemy || fallFloor) {
                        ObjectStomp os = null;  // 物体を踏んだ時の設定取得
                        // 子にアタッチしたルートオブジェクトを介してコンポーネントを取得する実験
                        try {
                            // ルートオブジェクトから取得トライ
                            os = collision.gameObject.GetComponent<RootReference>().obj.GetComponent<ObjectStomp>();
                        }
                        catch {
                            // 取得できない場合はコライダーと同じオブジェクトから取得
                            os = collision.gameObject.GetComponent<ObjectStomp>();
                        }
                        if (os != null) {
                            // 敵を踏んづけた時の処理
                            if (enemy) {
                                otherJumpHeight = os.boundHeight;   // 踏んづけたものから跳ねる高さを取得する
                                os.playerStompedOn = true;  // 踏んづけたものに対して踏んづけた事を通知する
                                jumpPos = transform.position.y; // ジャンプした位置を記録する
                                isBounce = true;
                                isJump = false;
                                jumpTime = 0.0f;
                            }
                            // 落ちる床を踏んだ時は相手に通知するだけ
                            else if (fallFloor) {
                                os.playerStompedOn = true;
                            }
                        }
                        else {
                            Debug.LogWarning($"[{collision.gameObject.name}] ObjectCollisionが設定されていません");
                        }
                    }
                    // 動く床を踏んだ時
                    else if (moveFloor) {
                        moveObj = collision.gameObject.GetComponent<MoveObjectIFCaller>();
                    }
                }
                // 通常の接触
                else {
                    if (enemy) {
                        ReceiveDamage(true, enemyTag);  // ダウンする
                        break;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.tag == moveFloorTag) {
            // 動く床から離れた
            moveObj = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 落下時はダウンアニメーションなし
        if (collision.tag == deadAreaTag) {
            ReceiveDamage(false, deadAreaTag);
        }
        // トゲ接触時はアニメーションあり
        else if (collision.tag == hitAreaTag) {
            ReceiveDamage(true, hitAreaTag);
        }
    }
    #endregion

    /// <summary>
    /// ミスしたときの処理
    /// </summary>
    /// <param name="downAnim">ダウンアニメーションするか</param>
    /// <param name="causeTag">ミス要因のタグ</param>
    private void ReceiveDamage(bool downAnim, string causeTag) {
        if (isDown || GManager.instance.isStageClear) {
            return;
        }
        else {
            if (downAnim) {
                anim.Play(animDown);
            }
            else {
                nonDownAnim = true;
            }
            isDown = true;
            AudioClip se = downSE;
            if (causeTag == deadAreaTag) se = fallSE;
            GManager.instance.PlaySE(se);
            GManager.instance.SubHeartNum();
        }
    }

    /// <summary>
    /// リスポーン待機状態か
    /// </summary>
    /// <returns>待機状態か否か</returns>
    public bool IsRespawnWaiting() {
        // ゲームオーバー時はリスポーンしない
        if (GManager.instance.isGameOver) {
            return false;
        }
        // ダウンアニメーション完了＆しない場合（落下など）にリスポーンできる
        else {
            return IsDownAnimEnd() || nonDownAnim;
        }
    }

    /// <summary>
    /// ダウンアニメーションが完了しているか
    /// </summary>
    /// <returns>アニメーションが完了しているか否か</returns>
    private bool IsDownAnimEnd() {
        if (isDown && anim != null) {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);   // 現在再生中のステートの情報を取得
            if (currentState.IsName(animDown)) {
                if (currentState.normalizedTime >= 1) { // アニメーションの正規化された再生時間（1が再生完了、※Animatorで矢印が出ている場合は使えない）
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// リスポーンする
    /// </summary>
    public void RespawnPlayer() {
        GManager.instance.PlaySE(respawnSE);
        // フラグ等をリセット
        isDown = false;
        anim.Play(animIdle);
        isRun = false;
        isJump = false;
        isJumpKeyPress = false;
        isJumpKeyRelease = false;
        isBounce = false;
        isLanding = false;
        isRespawn = true;  // 演出用フラグをON
        nonDownAnim = false;
    }

    /// <summary>
    /// リスポーンアニメーション（点滅）
    /// </summary>
    private void respawnAnimation() {
        if (isRespawn) {
            // 点滅 点いている時の戻る
            if (blinkTime > 0.2f) {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            // 点滅 消えている時
            else if (blinkTime > 0.1f) {
                sr.enabled = false;
            }
            // 点滅 点いている時
            else {
                sr.enabled = true;
            }
        }

        // 1秒たったら点滅終わり
        if (respawnTime > 1.0f) {
            isRespawn = false;
            blinkTime = 0.0f;
            respawnTime = 0.0f;
            sr.enabled = true;
        }
        else {
            blinkTime += Time.deltaTime;
            respawnTime += Time.deltaTime;
        }
    }
}
