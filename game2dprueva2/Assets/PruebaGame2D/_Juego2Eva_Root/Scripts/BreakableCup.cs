using UnityEngine;

public class BreakableCup : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite intactSprite;
    public Sprite brokenSprite;

    [Header("Curación")]
    public int healAmount = 1;

    [Header("FX")]
    public GameObject heartFXPrefab; // corazones volando
    public Transform fxSpawnPoint;

    private bool broken = false;
    private SpriteRenderer sr;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = intactSprite;
    }

    public void BreakCup()
    {
        Debug.Log("Copa rota!");

        if (broken) return;

        broken = true;
        sr.sprite = brokenSprite;

        Player player = FindObjectOfType<Player>();
        if (player == null) return;

        int currentHealth = player.CurrentHealth;
        int maxHealth = player.MaxHealth;

        // ✅ Solo curar si le falta vida
        if (currentHealth < maxHealth)
        {
            player.TakeHealing(healAmount);

            if (heartFXPrefab != null && fxSpawnPoint != null)
                Instantiate(heartFXPrefab, fxSpawnPoint.position, Quaternion.identity);
        }

        // destruir después de un pequeño delay para ver la rotura
        Destroy(gameObject, 0.4f);
    }

}
