using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel; // Panel de pausa
    private bool isPaused = false;

    [Header("Fade to Menu")]
    public CanvasGroup fadePanel; // Panel negro para fade
    public float fadeDuration = 0.9f;

    void Update()
    {
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
        // Aseguramos que el tiempo esté activo para el fade
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(true);
        StartCoroutine(FadeAndLoadAsync(mainMenuSceneName));
    }

    private IEnumerator FadeAndLoadAsync(string sceneName)
    {
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

        // Carga asíncrona de la escena, para que no quede congelado
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}