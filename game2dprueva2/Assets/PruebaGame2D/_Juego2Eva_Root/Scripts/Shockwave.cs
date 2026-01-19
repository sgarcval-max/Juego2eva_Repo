using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float damage = 1f;
    public float duration = 1.5f; // duración de la onda visible
    private bool hasHit = false;

    void Start()
    {
        // Destruye el shockwave automáticamente después de duration segundos
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage((int)damage);
            hasHit = true; // Para no dañar varias veces
            return;
        }

        // Si quieres que rompa paredes o interactúe con objetos
        // BreakableWall wall = collision.GetComponent<BreakableWall>();
        // if (wall != null) { wall.TakeHit(); hasHit = true; return; }
    }
}
