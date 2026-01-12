using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneTrigger2D_Fade : MonoBehaviour
{
    [Header("Player Settings")]
    public Player playerScript;               // Tu script Player
    public Vector2 moveDirection = Vector2.right; // Dirección de la cutscene
    public float moveSpeed = 3f;             // Velocidad del player

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;           // CanvasGroup negro
    public float fadeSpeed = 1f;             // Velocidad de fade

    [Header("Scene Settings")]
    public string nextSceneName;             // Nombre de la siguiente escena

    private bool cutsceneActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!cutsceneActive && other.CompareTag("Player"))
        {
            cutsceneActive = true;

            // Desactivar control del jugador
            if (playerScript != null)
                playerScript.EnableMovement(false);

            // Iniciar cutscene
            StartCoroutine(RunCutscene());
        }
    }

    private IEnumerator RunCutscene()
    {
        // Asegurarnos de que el fade comienza desde alpha 0
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = true;
        }

        while (fadeCanvas != null && fadeCanvas.alpha < 1f)
        {
            // Mover al jugador y actualizar animación
            if (playerScript != null)
                playerScript.PlayCutsceneMovement(moveDirection * moveSpeed);

            // Incrementar alpha del fade
            fadeCanvas.alpha += fadeSpeed * Time.unscaledDeltaTime;

            yield return null;
        }

        // Detener al jugador
        if (playerScript != null)
            playerScript.PlayCutsceneMovement(Vector2.zero);

        // Antes de cargar la escena, despause si estaba pausado
        Time.timeScale = 1f;

        // 🔑 Guardar la vida actual del jugador
        if (PlayerHealthManager.Instance != null && playerScript != null)
            PlayerHealthManager.Instance.UpdateHealth(playerScript.CurrentHealth);

        // Cargar la siguiente escena
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}