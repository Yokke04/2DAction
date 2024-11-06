using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージコントローラー
/// </summary>
public class StageCtrl : MonoBehaviour {
    [Header("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header("リスポーン地点")] public GameObject[] respawnPoint;
    [Header("ゲームオーバー")] public GameObject gameOverObj;
    [Header("ゲームクリア")] public GameObject gameClearObj;
    [Header("BGMオブジェクト")] public GameObject bgmObj;
    [Header("フェード")] public  FadeImage fade;
    [Header("ゲームオーバー時SE")] public AudioClip gameOverSE;
    [Header("コンティニュー時SE")] public AudioClip continueSE;
    [Header("ステージクリアSE")] public AudioClip stageClearSE;
    [Header("ゲームクリアSE")] public AudioClip gameClearSE;
    [Header("ステージクリア")] public GameObject stageClearObj;
    [Header("ステージクリア判定")] public PlayerTriggerCheck stageClearTrigger;
    [Header("カメラコライダー")] public GameObject cameraCollider;

    private Player p;
    private BGMManager bgmManager;
    private int nextStageNum;
    private bool startFade = false;
    private bool doGameOver = false;
    private bool doContinue = false;    // コンティニュー（現在ステージから）
    private bool doRestart = false;     // リスタート（ステージ1から）
    private bool doSceneChange = false;
    private bool doClear = false;

    void Start() {
        if (playerObj != null && respawnPoint != null && respawnPoint.Length > 0 && gameOverObj != null && gameClearObj != null && bgmObj != null && fade != null && cameraCollider != null) {
            gameOverObj.SetActive(false);
            gameClearObj.SetActive(false);
            stageClearObj.SetActive(false);
            playerObj.transform.position = respawnPoint[0].transform.position;
            cameraCollider.GetComponent<PolygonCollider2D>().enabled = false;   // 付けっぱなしだとキャラが落っこちるのでOFF

            p = playerObj.GetComponent<Player>();
            if (p == null) {
                Debug.LogWarning($"[{gameObject.name}] プレイヤーじゃないものがアタッチされています");
            }
            bgmManager = bgmObj.GetComponent<BGMManager>();
        }
        else {
            Debug.LogWarning($"[{gameObject.name}] インスペクターの設定が足りません");
        }
    }

    void Update() {
        // ゲームオーバー時の処理
        if (GManager.instance.isGameOver && !doGameOver) {
            gameOverObj.SetActive(true);
            GManager.instance.PlaySE(gameOverSE);
            bgmManager.StopBGM();
            doGameOver = true;
        }
        // プレイヤーがやられたときの処理
        else if (p != null && p.IsRespawnWaiting() && !doGameOver) {    // リスポーン待機状態
            if (respawnPoint.Length > GManager.instance.respawnNum) {   // リスポーンしたい位置の目印の設定が足りているか
                playerObj.transform.position = respawnPoint[GManager.instance.respawnNum].transform.position; // ゲームマネージャーで管理しているリスポーン地点番号
                p.RespawnPlayer();
            }
            else {
                Debug.LogWarning($"[{gameObject.name}] 中間地点の設定が足りません");
            }
        }
        // クリア判定に入ったら、クリア演出を行う
        else if (stageClearTrigger != null && stageClearTrigger.isOn && !doGameOver && !doClear) {
            StageClearAnimation();
            doClear = true;
        }

        // ステージを切り替える
        if (fade != null && startFade && !doSceneChange) {
            if (fade.IsFadeOutComplete()) {
                // ゲームコンティニューまたはリスタート
                if (doContinue || doRestart) {
                    GManager.instance.RestartGame(doContinue);
                }
                // 次のステージ
                else {
                    // 次のステージに備えて各種パラメータを設定する
                    GManager.instance.stageNum = nextStageNum;
                    GManager.instance.respawnNum = 0;
                    GManager.instance.isStageClear = false;
                }
                SceneManager.LoadScene("stage" + nextStageNum);
                doSceneChange = true;
            }
        }
    }

    /// <summary>
    /// 現在のステージから始める（コンティニュー）
    /// </summary>
    public void ContinueGame() {
        GManager.instance.PlaySE(continueSE);
        ChangeScene(GManager.instance.stageNum); // 現在のステージ
        doContinue = true;
    }

    /// <summary>
    /// 最初から始める（リスタート）
    /// </summary>
    public void RestartGame() {
        GManager.instance.PlaySE(continueSE);
        ChangeScene(1); // 最初のステージに戻るので1
        doRestart = true;
    }

    /// <summary>
    /// ステージクリア演出の実行（完了後にステージ遷移する）
    /// </summary>
    public void StageClearAnimation() {
        GManager.instance.isStageClear = true;
        stageClearObj.SetActive(true);
        bgmManager.StopBGM();
        GManager.instance.PlaySE(stageClearSE, true);
    }

    /// <summary>
    /// ステージを切り替える
    /// </summary>
    /// <param name="num">ステージ番号</param>
    public void ChangeScene(int num) {
        nextStageNum = num;
        fade.StartFadeOut();
        startFade = true;
    }

    /// <summary>
    /// ゲームクリア演出の実行（暫定）
    /// </summary>
    public void GameClear() {
        stageClearObj.SetActive(false);
        gameClearObj.SetActive(true);
        GManager.instance.PlaySE(gameClearSE);
    }

    /// <summary>
    /// ゲーム管理オブジェクトのアプリ終了処理を呼び出す
    /// </summary>
    public void QuitGame() {
        GManager.instance.QuitGame();
    }
}
