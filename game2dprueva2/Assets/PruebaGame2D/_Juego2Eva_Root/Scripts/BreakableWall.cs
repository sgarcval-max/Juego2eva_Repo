using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("Wall Settings")]
    [SerializeField] private int hitsToBreak = 3;

    [Header("Fragments")]
    [SerializeField] private GameObject fragmentsPrefab;
    [SerializeField] private float explosionForce = 4f;

    private int currentHits;

    // Este método lo llamará el ataque del player
    public void TakeHit()
    {
        currentHits++;

        if (currentHits >= hitsToBreak)
            BreakWall();
    }

    private void BreakWall()
    {
        // Instanciar trozos
        if (fragmentsPrefab != null)
        {
            GameObject fragments = Instantiate(
                fragmentsPrefab,
                transform.position,
                Quaternion.identity
            );

            foreach (Rigidbody2D rb in fragments.GetComponentsInChildren<Rigidbody2D>())
            {
                Vector2 forceDir = (rb.transform.position - transform.position).normalized;
                rb.AddForce(forceDir * explosionForce, ForceMode2D.Impulse);
            }
        }

        // Destruir pared
        Destroy(gameObject);
    }
}
