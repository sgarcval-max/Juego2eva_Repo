using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneTrigger2D_Fade : MonoBehaviour
{
    [Header("Player Settings")]
    public Player playerScript;       // Tu script Player
    public Vector2 moveDirection = Vector2.right; // Dirección de la cutscene
    public float moveSpeed = 3f;      // Velocidad del player

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;    // CanvasGroup negro
    public float fadeSpeed = 1f;      // Velocidad de fade

    [Header("Scene Settings")]
    public string nextSceneName;      // Nombre de la siguiente escena

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
        while (fadeCanvas.alpha < 1f)
        {
            // Llamar al método del player para moverlo y actualizar animación
            if (playerScript != null)
                playerScript.PlayCutsceneMovement(moveDirection * moveSpeed);

            // Incrementar alpha del fade
            if (fadeCanvas != null)
                fadeCanvas.alpha += fadeSpeed * Time.deltaTime;

            yield return null;
        }

        // Detener al player al final
        if (playerScript != null)
            playerScript.PlayCutsceneMovement(Vector2.zero);

        // Cargar la siguiente escena
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}