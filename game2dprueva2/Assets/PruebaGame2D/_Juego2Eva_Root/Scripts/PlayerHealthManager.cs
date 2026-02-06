using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance;

    [Header("UI Target (corazones voladores)")]
    public Transform heartTarget;

    [Header("Health Settings")]
    private int savedHealth = -1; // -1 = no inicializado
    private int maxHealth = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (savedHealth == -1)
            savedHealth = maxHealth;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.SetHealth(savedHealth);
        }
    }

    // =========================
    // 🔥 DAÑO
    // =========================
    public void TakeDamage(int amount)
    {
        savedHealth -= amount;

        if (savedHealth < 0)
            savedHealth = 0;

        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.SetHealth(savedHealth);

        Debug.Log("Jugador recibe daño. Vida: " + savedHealth);

        if (savedHealth <= 0)
        {
            PlayerDied();
        }
    }

    // =========================
    // 💚 CURAR
    // =========================
    public void Heal(int amount)
    {
        savedHealth += amount;

        if (savedHealth > maxHealth)
            savedHealth = maxHealth;

        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.SetHealth(savedHealth);

        Debug.Log("Jugador curado. Vida: " + savedHealth);
    }

    // =========================
    // ☠ MUERTE
    // =========================
    private void PlayerDied()
    {
        Debug.Log("Jugador muerto");

        // Reiniciar nivel
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // Guardado manual
    // =========================
    public void UpdateHealth(int currentHealth)
    {
        savedHealth = currentHealth;
    }

    public void ResetHealth()
    {
        savedHealth = maxHealth;

        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.SetHealth(maxHealth);
    }

    public int GetSavedHealth()
    {
        return savedHealth;
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
    }
}