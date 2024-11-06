using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// シーン切替フェードの制御
/// </summary>
public class FadeImage : MonoBehaviour {
    [Header("最初からフェードインが完了しているか")] public bool firstFadeInComp;

    private Image img = null;
    private int frameCount = 0;
    private float timer = 0.0f;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private bool compFadeIn = false;
    private bool compFadeOut = false;
    
    /// <summary>
    /// フェードインを開始する
    /// </summary>
    public void StartFadeIn() {
        if (fadeIn || fadeOut) {    // フェード中の重複開始無効化
            return;
        }
        fadeIn = true;
        compFadeIn = false;
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 1);
        img.fillAmount = 1;
        img.raycastTarget = true;
    }

    /// <summary>
    /// フェードインが完了したか
    /// </summary>
    public bool IsFadeInComplete() {
        return compFadeIn;
    }

    /// <summary>
    /// フェードアウトを開始する
    /// </summary>
    public void StartFadeOut() {
        if (fadeIn || fadeOut) {    // フェード中の重複開始無効化
            return;
        }
        fadeOut = true;
        compFadeOut = false;
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = true;
    }

    /// <summary>
    /// フェードアウトが完了したか
    /// </summary>
    public bool IsFadeOutComplete() {
        return compFadeOut;
    }


    void Start() {
        img = GetComponent<Image>();
        if (firstFadeInComp) {
            FadeInComplete();
        }
        else {
            StartFadeIn();
        }
    }

    void Update() {
        // シーン移行時の処理負荷でTime.deltaTimeが大きくなってしまうため2フレーム待つ
        if (frameCount > 2) {
            if (fadeIn) {
                FadeInUpdate();
            } 
            else if (fadeOut) {
                FaidOutUpdate();
            }
        }
        ++frameCount;
    }

    // フェードイン中
    private void FadeInUpdate() {
        if (timer < 1f) {
            img.color = new Color(1, 1, 1, 1 - timer); // Color(R, G, B, A) ここではアルファ操作を目的としている
            img.fillAmount = 1 - timer;
        }
        else {
            FadeInComplete();
        }
        timer += Time.deltaTime;
    }

    private void FaidOutUpdate() {
        if (timer < 1f) {
            img.color = new Color(1, 1, 1, timer);
            img.fillAmount = timer;
        }
        else {
            FadeOutComplete();
        }
        timer += Time.deltaTime;
    }

    // フェードイン完了
    private void FadeInComplete() {
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = false;  // ボタンの前面に配置されているため当たり判定を無効化する
        timer = 0.0f;
        fadeIn = false;
        compFadeIn = true;
    }

    // フェードアウト完了
    private void FadeOutComplete() {
        img.color = new Color(1, 1, 1, 1);
        img.fillAmount = 1;
        img.raycastTarget = false;
        timer = 0.0f;
        fadeOut = false;
        compFadeOut = true;
    }
}
