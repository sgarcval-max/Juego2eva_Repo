using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance;

    public Transform heartTarget;

    private int savedHealth = -1; // -1 significa que aún no se ha inicializado
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

        // Inicializamos vida al arrancar
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
