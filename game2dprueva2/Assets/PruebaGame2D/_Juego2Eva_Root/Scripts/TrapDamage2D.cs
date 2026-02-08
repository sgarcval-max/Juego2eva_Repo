using UnityEngine;

public class TrapDamage2D : MonoBehaviour
{
    public int damage = 1;

    private void OnColissionEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log (damage);
            }
        }
    }
}

