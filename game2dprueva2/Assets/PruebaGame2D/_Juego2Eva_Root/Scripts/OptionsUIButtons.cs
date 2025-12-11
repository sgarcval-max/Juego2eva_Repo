using UnityEngine;
using UnityEngine.UI;

public class OptionsUIButtons : MonoBehaviour
{
    [Header("Botones de Opciones")]
    public Button backButtonMain;            // Back principal al menú
    public Button backButtonSound;           // Back dentro del panel Sound
    public Button backButtonKeyboard;        // Back dentro del panel Keyboard
    public Button backButtonGamepad;         // Back dentro del panel Gamepad
    public Button backButtonKeyboardReassign; // Back dentro de reasignar teclado
    public Button backButtonGamepadReassign;  // Back dentro de reasignar gamepad

    public Button soundButton;
    public Button keyboardButton;
    public Button gamepadButton;
    public Button keyboardReassignButton;
    public Button gamepadReassignButton;

    private void Start()
    {
        // Asegurarse de que MenuControllerRuntime existe
        var mc = MenuControllerRuntime.Instance;
        if (mc == null)
        {
            Debug.LogError("[OptionsUIButtons] No existe MenuControllerRuntime en escena.");
            return;
        }

        // ------------------ Back Buttons ------------------
        if (backButtonMain != null)
            backButtonMain.onClick.AddListener(() => mc.ReturnToMainMenuFromOptions());

        if (backButtonSound != null)
            backButtonSound.onClick.AddListener(() => mc.ReturnToControlsPanel(mc.soundPanelGroup));

        if (backButtonKeyboard != null)
            backButtonKeyboard.onClick.AddListener(() => mc.ReturnToControlsPanel(mc.keyboardPanelGroup));

        if (backButtonGamepad != null)
            backButtonGamepad.onClick.AddListener(() => mc.ReturnToControlsPanel(mc.gamepadPanelGroup));

        if (backButtonKeyboardReassign != null)
            backButtonKeyboardReassign.onClick.AddListener(() => mc.CloseKeyboardReassignPanel());

        if (backButtonGamepadReassign != null)
            backButtonGamepadReassign.onClick.AddListener(() => mc.CloseGamepadReassignPanel());

        // ------------------ Otros Botones ------------------
        if (soundButton != null)
            soundButton.onClick.AddListener(() => mc.ShowSoundPanel());

        if (keyboardButton != null)
            keyboardButton.onClick.AddListener(() => mc.ShowKeyboardPanel());

        if (gamepadButton != null)
            gamepadButton.onClick.AddListener(() => mc.ShowGamepadPanel());

        if (keyboardReassignButton != null)
            keyboardReassignButton.onClick.AddListener(() => mc.OpenKeyboardReassignPanel());

        if (gamepadReassignButton != null)
            gamepadReassignButton.onClick.AddListener(() => mc.OpenGamepadReassignPanel());
    }
}