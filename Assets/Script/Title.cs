using System.Collections;
using System.Collections.Generic;
﻿using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面制御
/// </summary>
public class Title : MonoBehaviour {
    [Header("フェード")] public FadeImage fade;
    [Header("ゲームスタート時SE")] public AudioClip startSE;

    private bool firstPush = false; // ボタン連打防止
    private bool goNextScene = false;

    public void Update() {
        if (!goNextScene && fade.IsFadeOutComplete()) { // フェードアウトの完了を監視
            SceneManager.LoadScene("Stage1");
            goNextScene = true;
        }
    }

    // スタートボタンを押されたら呼ばれる
    public void PressStart() {
        if (!firstPush) {
            // 次のシーンに行く命令
            GManager.instance.PlaySE(startSE);
            fade.StartFadeOut();
            firstPush = true;
        }
    }

    /// <summary>
    /// ゲーム管理オブジェクトのアプリ終了処理を呼び出す
    /// </summary>
    public void QuitGame() {
        GManager.instance.QuitGame();
    }
}
