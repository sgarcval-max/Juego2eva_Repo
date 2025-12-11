using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;     // Panel de pausa
    public CanvasGroup fadePanel;         // Panel negro para fade

    [Header("Fade Settings")]
    public float fadeDuration = 1f;       // Duración del fade

    private bool isPaused = false;

    void Update()
    {
        // Toggle pausa con Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

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

    // Volver al menú con fade
    public void ReturnToMainMenu(string menuSceneName)
    {
        StartCoroutine(FadeAndLoad(menuSceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Aseguramos que el fadePanel está activo
        fadePanel.gameObject.SetActive(true);
        fadePanel.alpha = 0f;

        // Mantener la pausa mientras se hace el fade
        Time.timeScale = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            fadePanel.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 1f;

        // Guardar flag para saltar intro en el Main Menu
        PlayerPrefs.SetInt("SkipIntro", 1);

        // Restaurar tiempo y cargar escena
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

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
        Debug.Log("Abrir opciones (añade tu panel de opciones aquí)");
    }
}
