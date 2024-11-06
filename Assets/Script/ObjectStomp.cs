using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクト踏みつけ情報管理
/// </summary>
public class ObjectStomp : MonoBehaviour {
    [Header("これを踏んだ時のプレイヤーが跳ねる高さ")] public float boundHeight;
    
    // このオブジェクトをプレイヤーが踏んだかどうか
    [HideInInspector] public bool playerStompedOn; // インスペクターにシリアライズされたものが表示されなくなる
}
