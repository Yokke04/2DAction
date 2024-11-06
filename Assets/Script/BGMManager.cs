using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM制御
/// </summary>
public class BGMManager : MonoBehaviour {

    [Header("再生時間変更を使用する")] public bool useTimeChange = true;
    [Header("BGMの再生開始時間（0以下は変更なし)")] public float playStartTime = 0.0f;
    [Header("BGMのループ開始時間（0以下は変更なし)")] public float loopStartTime = 0.0f;
    [Header("BGMのループ終了時間（0以下は変更なし)")] public float loopEndTime = 0.0f;

    private AudioSource audioSource;
    private bool useFixTime = false;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        if (useTimeChange && audioSource != null) {
            // Play On Awakeが有効な場合、再生を停止しておく
            if (audioSource.playOnAwake) {
                audioSource.Stop();
            }
        }
    }

    private void Start() {
        if (audioSource.clip == null) {
            Debug.LogWarning($"[{gameObject.name}] オーディオクリップが設定されていません");
            Destroy(gameObject);
            return;
        }

        if (useTimeChange) {
            // インスペクター指定時間の補正
            FixStartTime();
            FixLoopTime();

            if (useFixTime) {
                audioSource.time = playStartTime;
                audioSource.Play();
            }
            else if (audioSource.playOnAwake) {
                // Play On Awakeが有効で、入力値に変更がない場合はそのまま再生
                audioSource.Play();
            }
        }
    }

    private void Update() {
        if (useTimeChange) {
            // ループ時間の途中変更に対応して補正
            FixLoopTime();

            // ループ時間指定されている場合
            if (useFixTime && audioSource != null && audioSource.isPlaying) {
                if (audioSource.time > loopEndTime) {
                    audioSource.time = loopStartTime;
                }
            }
        }
    }

    /// <summary>
    /// 再生時間の不正値を修正する
    /// </summary>
    private void FixStartTime() {
        // 再生開始時間
        if (playStartTime >= loopEndTime && playStartTime > 0.0f && loopEndTime > 0.0f) {
            Debug.Log("BGMの再生開始時間に再生終了時間以上の長さが指定されている為、時間変更を無効化します。");
            playStartTime = 0.0f;
        }
        else if (playStartTime >= audioSource.clip.length) {
            Debug.Log("BGMの再生開始時間にオーディオソース以上の長さが指定されている為、時間変更を無効化します。");
            playStartTime = 0.0f;
        }
    }

    /// <summary>
    /// ループ時間・終了時間の不正値を修正する
    /// </summary>
    private void FixLoopTime() {
        // ループ開始時間
        if (loopStartTime >= loopEndTime && loopStartTime > 0.0f && loopEndTime > 0.0f) {
            Debug.Log("BGMのループ開始時間に再生終了時間以上の長さが指定されている為、時間変更を無効化します。");
            loopStartTime = 0.0f;
        }
        else if (loopStartTime >= audioSource.clip.length) {
            Debug.Log("BGMのループ開始時間にオーディオソース以上の長さが指定されている為、時間変更を無効化します。");
            loopStartTime = 0.0f;
        }

        // ループ終了時間
        if (loopEndTime > audioSource.clip.length || loopEndTime == 0.0f) {
            if (loopEndTime > audioSource.clip.length) {
                Debug.Log("BGMのループ終了時間にオーディオソースより長い時間が指定されている為、時間変更を無効化します。");
            }
            loopEndTime = audioSource.clip.length;
        }

        // 時間変更機能を使用するかどうかの判定
        useFixTime = (playStartTime > 0.0f || loopStartTime > 0.0f || loopEndTime < audioSource.clip.length);
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="startTime">再生開始時間を指定(任意)</param>
    public void PlayBGM(float startTime = -1.0f) {
        if (startTime > 0.0f) {
            playStartTime = startTime;
            FixStartTime();
            audioSource.time = playStartTime;
        }
        else {
            audioSource.Play();
        }
    }

    /// <summary>
    /// BGMを止める
    /// </summary>
    public void StopBGM() {
        audioSource.Stop();
    }
}
