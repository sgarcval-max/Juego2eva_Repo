using UnityEngine;
using UnityEngine.UI;

public class OptionsUIButtons : MonoBehaviour
{
    [Header("Opciones UI Buttons")]
    public Button controlsButton;
    public Button soundButton;
    public Button keyboardButton;
    public Button gamepadButton;
    public Button keyboardReassignButton;
    public Button gamepadReassignButton;
    public Button backButton; // Volver al Main Menu desde Options

    private void Awake()
    {
        // Comprobamos que existe el singleton
        if (MenuControllerRuntime.Instance == null)
        {
            Debug.LogError("[OptionsUIButtons] No existe MenuControllerRuntime en escena.");
            return;
        }

        // Asignamos los listeners de los botones
        if (controlsButton != null)
            controlsButton.onClick.AddListener(() => MenuControllerRuntime.Instance.ShowControlsPanel());

        if (soundButton != null)
            soundButton.onClick.AddListener(() => MenuControllerRuntime.Instance.ShowSoundPanel());

        if (keyboardButton != null)
            keyboardButton.onClick.AddListener(() => MenuControllerRuntime.Instance.ShowKeyboardPanel());

        if (gamepadButton != null)
            gamepadButton.onClick.AddListener(() => MenuControllerRuntime.Instance.ShowGamepadPanel());

        if (keyboardReassignButton != null)
            keyboardReassignButton.onClick.AddListener(() => MenuControllerRuntime.Instance.OpenKeyboardReassignPanel());

        if (gamepadReassignButton != null)
            gamepadReassignButton.onClick.AddListener(() => MenuControllerRuntime.Instance.OpenGamepadReassignPanel());

        if (backButton != null)
            backButton.onClick.AddListener(() => MenuControllerRuntime.Instance.ReturnToMainMenuFromOptions());
    }
}
