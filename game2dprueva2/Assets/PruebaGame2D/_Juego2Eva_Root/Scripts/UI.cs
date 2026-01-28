using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] private GameObject gameOverUI;
    [Space]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killCountText;

    private int killCount;

    [Header("Game Over Settings")]
    [SerializeField] private float slowMotionTime = 0.5f; // velocidad al morir

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (timerText != null)
            timerText.text = Time.time.ToString("F2") + "s";
    }

    public void EnableGameOverUI()
    {
        // Slow motion al morir
        Time.timeScale = slowMotionTime;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // ajustar física

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public void RestartLevel()
    {
        // Reset de vida al reiniciar
        if (PlayerHealthManager.Instance != null)
        {
            PlayerHealthManager.Instance.ResetHealth();
        }

        // Restaurar velocidad normal
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public void AddKillCount()
    {
        killCount++;
        if (killCountText != null)
            killCountText.text = killCount.ToString();
    }
}
