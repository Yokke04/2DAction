using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 中間地点
/// </summary>
public class CheckPoint : MonoBehaviour {
    [Header("中間地点番号")] public int checkPointNum;
    [Header("音")] public AudioClip se;
    [Header("プレイヤー判定")] public PlayerTriggerCheck trigger;
    //[Header("通過アニメーションカーブ")] public AnimationCurve curve;
    //[Header("通過アニメーションのスピード")] public float speed = 2.0f;

    Animator anim = null;
    private bool on;
    private float timer;
    //private float maxCurveTime; // アニメーションカーブの最大値

    void Start() {
        if (trigger == null || se == null) {
            Debug.LogWarning($"[{gameObject.name}] インスペクターの設定が足りません");
            Destroy(this);
        }
        anim = GetComponentInChildren<Animator>();
        //maxCurveTime = curve.keys[curve.length - 1].time;
    }

    void Update() {
        // プレイヤーが範囲内に入ったら通過演出開始
        if (trigger.isOn && !on) {
            GManager.instance.respawnNum = checkPointNum;
            GManager.instance.PlaySE(se);
            anim.SetBool("passed", true);
            on = true;
        }

        #region //// 消える演出
        //if (on) {
        //    // 進行中
        //    if (timer < maxCurveTime) {
        //        transform.localScale = Vector3.one * curve.Evaluate(timer);
        //        timer += speed * Time.deltaTime;
        //    }
        //    // 終了
        //    else {
        //        transform.localScale = Vector3.one * curve.Evaluate(maxCurveTime);
        //        gameObject.SetActive(false);
        //        on = false;
        //    }
        //}
        #endregion
    }
}
