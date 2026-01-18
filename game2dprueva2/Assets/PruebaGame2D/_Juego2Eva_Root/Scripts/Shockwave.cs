using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float speed = 2f;
    public float lifeTime = 10f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

    // Para usar con Animation Event si quieres
    public void DestroyShockwave()
    {
        Destroy(gameObject);
    }
}
