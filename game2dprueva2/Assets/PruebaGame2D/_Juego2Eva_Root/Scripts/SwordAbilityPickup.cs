using UnityEngine;

public class SwordAbilityPickup : MonoBehaviour
{
    [Header("Referencia al Player")]
    public Player player;

    [Header("Opcional: mostrar mensaje")]
    public GameObject pickupUI;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (other.CompareTag("Player"))
        {
            collected = true;

            if (pickupUI != null)
                pickupUI.SetActive(true);

            if (player != null)
                player.EnableMovement(false);

            SwordCinematicManager.instance.PlaySwordCinematic(() =>
            {
                if (player != null)
                    player.UnlockAbility(AbilityType.MeleeAttack);

                if (player != null)
                    player.EnableMovement(true);

                if (pickupUI != null)
                    pickupUI.SetActive(false);

                Destroy(gameObject);
            });
        }
    }
}