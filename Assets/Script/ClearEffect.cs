using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// クリアアニメーション制御
/// </summary>
public class ClearEffect : MonoBehaviour {
    [Header("拡大縮小のアニメーションカーブ")] public AnimationCurve curve;
    [Header("ステージコントローラー")] public StageCtrl ctrl;

    private bool comp;
    private float timer;
    
    void Start() {
        transform.localScale = Vector3.zero;
    }

    void Update() {
        if (!comp) {
            if (timer < 1.0f) {
                transform.localScale = Vector3.one * curve.Evaluate(timer);
                timer += Time.deltaTime;
            }
            else {
                if (GManager.instance.stageNum >= GManager.instance.finalStageNum) {
                    if (GManager.instance.isCompleteSE == true) {
                        ctrl.GameClear();
                    }
                }
                else {
                    transform.localScale = Vector3.one;
                    ctrl.ChangeScene(GManager.instance.stageNum + 1);
                    comp = true;
                }
            }
        }
    }
}
