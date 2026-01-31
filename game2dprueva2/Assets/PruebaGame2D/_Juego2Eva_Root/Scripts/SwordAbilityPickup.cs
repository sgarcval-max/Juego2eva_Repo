using UnityEngine;

public class SwordAbilityPickup : MonoBehaviour
{
    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        pickedUp = true;

        // Reproducimos la cinemática y desbloqueamos la espada al terminar
        SwordCinematicManager.Instance.PlaySwordCinematic(() =>
        {
            player.UnlockAbility(AbilityType.MeleeAttack);

            // Destruimos el pickup
            Destroy(gameObject);
        });
    }
}