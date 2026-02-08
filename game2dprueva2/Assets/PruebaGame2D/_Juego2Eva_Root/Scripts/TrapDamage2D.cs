using UnityEngine;

public class TrapDamage2D : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Daño aplicado: " + damage);
            }
        }
    }
}

