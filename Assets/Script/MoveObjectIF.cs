using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動する床の制御スクリプトのインターフェース
/// </summary>
public interface MoveObjectIF {
    Vector2 GetVelocity();
}