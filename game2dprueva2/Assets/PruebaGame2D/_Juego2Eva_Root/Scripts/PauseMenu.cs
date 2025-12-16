using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuPanel;

    [Header("Fade to Menu")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 0.9f;

    private bool isPaused = false;

    [Header("Input")]
    [SerializeField] private string pauseActionName = "Pause / Menu";

    void Update()
    {
        if (!isPaused && PauseInputPressed())
        {
            PauseGame();
        }
    }

    // ---------------- INPUT ----------------
    private bool PauseInputPressed()
    {
        if (KeyBindingsManager.Instance == null) return false;

        // Detectar si hay gamepad
        if (KeyBindingsManager.Instance.IsGamepadConnected())
        {
            // Botón gamepad
            KeyCode gamepadKey = KeyBindingsManager.Instance.GetBinding(
                pauseActionName,
                InputDeviceType.Gamepad
            );

            if (gamepadKey != KeyCode.None && Input.GetKeyDown(gamepadKey))
                return true;

            // Axis gamepad (opcional)
            string axis = KeyBindingsManager.Instance.GetGamepadAxisBinding(pauseActionName);
            if (!string.IsNullOrEmpty(axis) && Mathf.Abs(Input.GetAxisRaw(axis)) > 0.5f)
                return true;
        }

        // Teclado
        KeyCode keyboardKey = KeyBindingsManager.Instance.GetBinding(
            pauseActionName,
            InputDeviceType.Keyboard
        );

        return keyboardKey != KeyCode.None && Input.GetKeyDown(keyboardKey);
    }

    // ---------------- PAUSE ----------------
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // ---------------- BUTTONS ----------------
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OpenOptions()
    {
        Debug.Log("Abrir opciones");
    }

    public void ReturnToMainMenu(string mainMenuSceneName)
    {
        StartCoroutine(FadeAndLoad(mainMenuSceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Mantener el juego pausado mientras se hace el fade
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true;
            fadePanel.alpha = 0f;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                fadePanel.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }

            fadePanel.alpha = 1f;
        }

        // 🔑 IMPORTANTE: despause JUSTO antes de cambiar de escena
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}