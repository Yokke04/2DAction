using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ルートオブジェクトと自オブジェクト(子孫)を直接参照設定で紐づける
/// </summary>
public class RootReference : MonoBehaviour {

    [Header("ルートオブジェクト")] public GameObject obj;
    void Start() {
        if (obj == null) {
            Debug.LogWarning($"[{GManager.GetHierarchyPath(gameObject)}] ルートオブジェクトが設定されていません");
            Destroy(this.gameObject);
        }
    }
}
