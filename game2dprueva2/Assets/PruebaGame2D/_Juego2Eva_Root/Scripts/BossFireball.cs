using UnityEngine;

public class BossFireball : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 1f;   // puede ser decimal
    public float lifeTime = 5f;

    private Transform target;   // El jugador
    private Vector2 direction;

    void Start()
    {
        // Busca al jugador automáticamente
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target != null)
        {
            // Dirección hacia el jugador
            direction = (target.position - transform.position).normalized;

            // Flip visual según dirección
            if (direction.x < 0 && transform.localScale.x > 0)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
            else if (direction.x > 0 && transform.localScale.x < 0)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }

            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            player.TakeDamage(Mathf.CeilToInt(damage)); // aplica daño al jugador
            Destroy(gameObject);
            return;
        }

        // Si golpea el suelo o paredes rompibles
        BreakableWall wall = collision.GetComponent<BreakableWall>();
        if (wall != null)
        {
            wall.TakeHit();
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
