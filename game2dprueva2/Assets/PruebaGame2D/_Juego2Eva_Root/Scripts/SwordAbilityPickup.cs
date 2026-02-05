using UnityEngine;

public class SwordAbilityPickup : MonoBehaviour
{
    [Header("Referencia al Player")]
    public Player player;

    [Header("Referencia al Cinematic Manager")]
    public SwordCinematicManager cinematicManager;

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

            if (cinematicManager != null)
            {
                cinematicManager.PlaySwordCinematic(() =>
                {
                    if (player != null)
                        player.UnlockAbility(AbilityType.MeleeAttack);

                    // 🔥 ACTUALIZAR UI DE HABILIDADES con el nuevo método
                    if (AbilityUIManager.Instance != null)
                        AbilityUIManager.Instance.UnlockAbility(2); // espada = icono 3 (index 2)

                    if (player != null)
                        player.EnableMovement(true);

                    if (pickupUI != null)
                        pickupUI.SetActive(false);

                    Destroy(gameObject);
                });
            }
            else
            {
                Debug.LogWarning("Cinematic Manager no asignado en SwordAbilityPickup");

                if (player != null)
                    player.UnlockAbility(AbilityType.MeleeAttack);

                if (AbilityUIManager.Instance != null)
                    AbilityUIManager.Instance.UnlockAbility(2); // espada = icono 3 (index 2)

                if (player != null)
                    player.EnableMovement(true);

                if (pickupUI != null)
                    pickupUI.SetActive(false);

                Destroy(gameObject);
            }
        }
    }
}