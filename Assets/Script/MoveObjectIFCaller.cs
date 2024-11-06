using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動する床の制御スクリプトのインターフェース呼び出し用
/// </summary>
public class MoveObjectIFCaller : MonoBehaviour {
    [Header("親オブジェクト")] public GameObject parentObj;


    private MoveObjectIF moveObjectIF;

    void Start() {
        if (parentObj != null) {
            moveObjectIF = parentObj.GetComponent<MoveObjectIF>();
        }
        else {
            Debug.LogWarning($"[{gameObject.name}] インスペクターの設定が足りません");
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 動いた距離を返す
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVelocity() {
        return moveObjectIF.GetVelocity();
    }
}
