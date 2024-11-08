﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 判定内のプレイヤーチェック
/// </summary>
public class PlayerTriggerCheck : MonoBehaviour {
    [HideInInspector] public bool isOn = false; // 判定内にプレイヤーがいる

    private string playerTag = "Player";

    // 接触判定
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == playerTag) {
            isOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == playerTag) {
            isOn = false;
        }
    }
}
