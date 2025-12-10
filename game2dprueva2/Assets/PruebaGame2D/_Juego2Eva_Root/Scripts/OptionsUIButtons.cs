using UnityEngine;
using UnityEngine.UI;

public class OptionsUIButtons : MonoBehaviour
{
    [Header("Botones de Opciones")]
    public Button backButton;
    public Button soundButton;
    public Button keyboardButton;
    public Button gamepadButton;
    public Button keyboardReassignButton;
    public Button gamepadReassignButton;

    private void Start()
    {
        // Asegurarse de que MenuControllerRuntime existe
        if (MenuControllerRuntime.Instance == null)
        {
            Debug.LogError("[OptionsUIButtons] No existe MenuControllerRuntime en escena.");
            return;
        }

        // Asignar listeners a los botones
        if (backButton != null)
            backButton.onClick.AddListener(() => MenuControllerRuntime.Instance.ReturnToMainMenuFromOptions());

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
    }
}
