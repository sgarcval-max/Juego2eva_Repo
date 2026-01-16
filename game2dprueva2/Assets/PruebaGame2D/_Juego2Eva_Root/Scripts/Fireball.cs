using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifeTime = 3f;

    [Header("Cooldown Settings")]
    public float fireCooldown = 1f; // tiempo en segundos entre disparos

    private Vector2 direction;

    // Control del cooldown (estático para todos los fireballs)
    private static float lastFireTime = 0f;

    // Método para saber si se puede disparar
    public bool CanFire()
    {
        return Time.time >= lastFireTime + fireCooldown;
    }

    // Método para registrar el último disparo
    public void RegisterFire()
    {
        lastFireTime = Time.time;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si golpea a un enemigo
        Entity entity = collision.GetComponent<Entity>();
        if (entity != null && !(entity is Player))
        {
            entity.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Si golpea una pared rompible
        BreakableWall wall = collision.GetComponent<BreakableWall>();
        if (wall != null)
        {
            wall.TakeHit();
            Destroy(gameObject);
            return;
        }

        // Si golpea cualquier cosa sólida
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}