using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel; // Panel de pausa
    private bool isPaused = false;

    [Header("Fade to Menu")]
    public CanvasGroup fadePanel; // Panel negro para fade
    public float fadeDuration = 0.9f;

    void Update()
    {
        // Solo abrir el menú con ESC, no cerrar
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
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
        Debug.Log("Abrir opciones (puedes agregar tu panel de opciones aquí)");
    }

    public void ReturnToMainMenu(string mainMenuSceneName)
    {
        // Mantener menú activo durante el fade
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 1f; // Necesario para que corran los fades
        StartCoroutine(FadeAndLoad(mainMenuSceneName));
    }

    private System.Collections.IEnumerator FadeAndLoad(string sceneName)
    {
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                fadePanel.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = 1f;
        }

        // Cargar escena del menú
        SceneManager.LoadScene(sceneName);
    }
}
