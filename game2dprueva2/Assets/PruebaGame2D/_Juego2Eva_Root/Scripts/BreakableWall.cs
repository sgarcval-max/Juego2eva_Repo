using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public int hitsToBreak = 3;          // Total golpes para romper
    private int currentHits = 0;

    [Header("Sprites")]
    public Sprite[] crackedSprites;       // Los sprites de daño progresivo (0 = sin daño)
    private SpriteRenderer sr;

    [Header("Broken Pieces")]
    public GameObject brokenPrefab;       // Prefab con los trozos que caen

    private Collider2D wallCollider;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();

        // Asegurarse de que el primer sprite sea el correcto
        if (crackedSprites.Length > 0)
            sr.sprite = crackedSprites[0];
    }

    public void TakeHit()
    {
        currentHits++;

        // Cambiar sprite según los golpes
        if (crackedSprites.Length > 0 && currentHits - 1 < crackedSprites.Length)
        {
            sr.sprite = crackedSprites[currentHits - 1];
        }

        if (currentHits >= hitsToBreak)
        {
            BreakWall();
        }
    }

    private void BreakWall()
    {
        // Instanciar los pedazos
        if (brokenPrefab != null)
        {
            GameObject pieces = Instantiate(brokenPrefab, transform.position, transform.rotation);

            // Añadir fuerza aleatoria a cada pedazo
            Rigidbody2D[] rbs = pieces.GetComponentsInChildren<Rigidbody2D>();
            foreach (Rigidbody2D rb in rbs)
            {
                // Fuerza aleatoria en X e Y
                float forceX = Random.Range(-2f, 2f);
                float forceY = Random.Range(2f, 5f);
                rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);

                // Rotación aleatoria
                float torque = Random.Range(-10f, 10f);
                rb.AddTorque(torque, ForceMode2D.Impulse);
            }
        }

        // Desactivar colisión
        if (wallCollider != null)
            wallCollider.enabled = false;

        // Desactivar la pared original
        gameObject.SetActive(false);
    }
}
