using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーアニメーション制御
/// </summary>
public class PlayerAnimation : MonoBehaviour {
    // イベント通知用
    public delegate void AnimationAction();    // デリゲート
    public event AnimationAction OnLandingStart;
    public event AnimationAction OnLandingComplete;

    // *** イベント発火 ***
    /// <summary>
    /// 着地アニメーション開始(着地中フラグON)
    /// </summary>
    private void LandingStart() {
        // イベント通知
        if (OnLandingStart != null) {
            OnLandingStart();
       }
    }

    /// <summary>
    /// 着地アニメーション完了(着地中フラグOFF)
    /// </summary>
    private void LandingComplete() {
        // イベント通知
        if (OnLandingComplete  != null) {
            OnLandingComplete();
        }
    }
}
