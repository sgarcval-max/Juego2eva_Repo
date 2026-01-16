using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1;
    public float lifeTime = 3f;

    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        direction = dir;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity enemy = collision.GetComponent<Entity>();

        if (enemy != null && !(enemy is Player))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}