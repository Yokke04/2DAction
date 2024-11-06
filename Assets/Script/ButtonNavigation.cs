using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// キー入力でボタン制御
/// 上下で選択、スペース・Enterで決定
/// </summary>
public class ButtonNavigation : MonoBehaviour {
    [Header("ボタン")]　public Button[] buttons;
    private int currentIndex = 0;

    void Start() {
        if (buttons != null && buttons.Length > 0) {
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
        else {
            Debug.LogError("ボタンがアタッチされていません");
        }
    }

    void Update() {
        var keyboard = Keyboard.current;

        if (keyboard.upArrowKey.wasPressedThisFrame) {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame) {
            currentIndex = (currentIndex + 1) % buttons.Length;
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }

        if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame) {
            buttons[currentIndex].onClick.Invoke();
        }

        // フォーカスを強制的に設定し直す
        if (EventSystem.current.currentSelectedGameObject == null) {
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
    }
}
