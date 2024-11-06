using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ゲーム管理
/// </summary>
public class GManager : MonoBehaviour {
    public static GManager instance = null;

    [Header("スコア")]public int score;
    [Header("現在のステージ")] public int stageNum;
    [Header("最終ステージ")] public int finalStageNum;
    [Header("現在のリスポーン地点")]public int respawnNum;
    [Header("現在の残機")]public int remPlayers;
    [Header("デフォルトの残機")] public int defaultRemPlayers;
    [HideInInspector] public bool isGameOver;
    [HideInInspector] public bool isStageClear;
    [HideInInspector] public bool isCompleteSE;

    private AudioSource audioSource = null;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 残機を増やす
    /// </summary>
    public void AddHeartNum() {
        if (remPlayers < 99) {    // 上限を超えないように
            ++remPlayers;
        }
    }

    /// <summary>
    /// 残機を1減らす
    /// </summary>
    public void SubHeartNum() {
        if (remPlayers > 0) {
            --remPlayers;
        }
        else {
            isGameOver = true;
        }
    }

    /// <summary>
    /// ステージを最初から始める時の処理
    /// </summary>
    /// <param name="doContinue">コンティニューするか（省略時はステージ1から）</param>
    public void RestartGame(bool doContinue = false) {
        isGameOver = false;
        remPlayers = defaultRemPlayers;
        score = 0;
        stageNum = doContinue ? stageNum : 1;
        respawnNum = 0;
    }

    /// <summary>
    /// SEを鳴らす
    /// </summary>
    /// <param name="clip">鳴らすオーディオクリップ</param>
    /// <param name="CheckComplete">再生終了時のフラグを立てるか</param>
    public void PlaySE(AudioClip clip, bool checkComplete = false) {
        if (audioSource != null) {
            audioSource.PlayOneShot(clip);
            if (checkComplete) {
                isCompleteSE = false;
                StartCoroutine(CheckIfPlaying(clip.length));
            }
        }
        else {
            Debug.LogWarning($"[{gameObject.name}] オーディオソースが設定されていません");
        }
    }

    /// <summary>
    /// PlayOneShotで再生されたSEの終了をフラグ立てするコルーチン
    /// </summary>
    /// <param name="clipLength"></param>
    /// <returns></returns>
    private IEnumerator CheckIfPlaying(float clipLength) {
        yield return new WaitForSeconds(clipLength);
        isCompleteSE = true;
    }

    /// <summary>
    /// ゲームアプリ終了
    /// </summary>
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディターでのプレイを終了
#else
        Application.Quit(); // ビルドしたゲームを終了
#endif
    }

    /// <summary>
    /// オブジェクトのパスを返す
    /// </summary>
    /// <param name="obj">対象オブジェクト</param>
    /// <param name="root">パスのルートとする親オブジェクト(任意)</param>
    /// <param name="separator">区切り文字(任意)</param>
    /// <returns></returns>
    // rootとseparatorを指定するメソッド
    public static string GetHierarchyPath(GameObject obj, GameObject root = null, string separator = "\\") {
        return GetHierarchyPath(obj.transform, root?.transform, separator);
    }

    // separatorのみを指定するメソッド
    public static string GetHierarchyPath(GameObject obj, string separator) {
        return GetHierarchyPath(obj.transform, null, separator);
    }

    // rootとseparatorを指定しないメソッド
    public static string GetHierarchyPath(GameObject obj) {
        return GetHierarchyPath(obj.transform, null, "\\");
    }

    private static string GetHierarchyPath(Transform obj, Transform root, string separator) {
        if (obj == null) {
            return string.Empty;
        }
        if (obj == root) {
            return obj.name;
        }
        string parentPath = GetHierarchyPath(obj.parent, root, separator);
        return string.IsNullOrEmpty(parentPath) ? obj.name : parentPath + separator + obj.name;
    }
}
