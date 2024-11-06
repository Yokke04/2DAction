using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 背景画像の視差スクロール制御
/// </summary>
public class BackgroundScroll : MonoBehaviour {
    [SerializeField, Header("視差効果（大きいほどゆっくり）"), Range(0, 1)] private float parallaxEffect;

    private GameObject cam;
    private float length;
    [Header("背景座標X")] public float startPosX;
    
    
    void Start() {
        startPosX  = transform.position.x;  // 背景画像のx座標
        length = GetComponent<SpriteRenderer>().bounds.size.x;  // 背景画像のx軸方向の幅
        cam = Camera.main.gameObject;
    }

    void FixedUpdate() {
        Parallax();
    }

    private void Parallax() {
        float temp = cam.transform.position.x * (1 - parallaxEffect);   // 無限スクロールに使用するパラメーター
        float dist = cam.transform.position.x * parallaxEffect; // 背景の視差効果に使用するパラメーター
        // 視差効果を与える処理
        transform.position = new Vector3(startPosX + dist, transform.position.y, transform.position.z); // 背景画像のx座標をdistの分移動させる

        // 無限スクロール(画面外になったら背景画像を移動させる)
        if (temp > startPosX  + length) {
            startPosX += length;
        }
        else if (temp < startPosX - length) {
            startPosX -= length;
        }
    }
}
