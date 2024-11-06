using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地面の設置判定
/// </summary>
public class GroundCheck : MonoBehaviour {
    [Header("エフェクトがついた床を判定するか")] public bool checkPlatformGround;

    private string groundTag = "Ground";
    private string platformTag = "GroundPlatform";
    private string moveFloorTag = "MoveFloor";
    private string fallFloorTag = "FallFloor";
    private bool isGround = false;
    private bool isGroundEnter, isGroundStay, isGroundExit;

    /*// イベント通知用
    public delegate void TriggerEnterAction(Collider2D collision);    // デリゲート
    public event TriggerEnterAction OnTriggerEnterEvent; // イベント*/

    /// <summary>
    /// 複数のオブジェクトの判定に同時に接地した際の対応
    /// ※物理判定の更新毎に呼ぶ必要がある
    /// </summary>
    /// <returns></returns>
    public bool IsGround() {
        if (isGroundEnter || isGroundStay) {
            isGround = true;
        }
        else if (isGroundExit) {
            isGround = false;
        }

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        isGroundEnter = CheckCollisionEnter(collision);
        //if (isGroundEnter) Debug.Log("地面が判定に入りました");
        /*if (isGroundEnter) {
            // イベント通知
            if (OnTriggerEnterEvent != null) {
                OnTriggerEnterEvent(collision);
            }
        }*/
    }

    private void OnTriggerStay2D(Collider2D collision) {
        isGroundStay = CheckCollisionEnter(collision);
        //if (isGroundStay) Debug.Log("地面が判定に入り続けています");
    }

    private void OnTriggerExit2D(Collider2D collision) {
        isGroundExit = CheckCollisionEnter(collision);
        //if (isGroundExit) Debug.Log("地面が判定を出ました");
    }

    /// <summary>
    /// 地面にしたかを返す（対象：マップタイル、プラットフォーム）
    /// </summary>
    /// <param name="collision"></param>
    /// <returns></returns>
    private bool CheckCollisionEnter(Collider2D collision) {
        // マップタイル判定
        if (collision.tag == groundTag) {
            return true;
        }
        // プラットフォーム判定
        else if (checkPlatformGround && (collision.tag == platformTag || collision.tag == moveFloorTag || collision.tag == fallFloorTag)) {
            return true;
        }
        return false;
    }
}
