using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager Instance;

    [Header("UI")]
    public GameObject pickupPanel;
    public ItemPopupUI pickupUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectItem(ItemData item)
    {
        if (item == null) return;

        // Pausa el juego
        Time.timeScale = 0f;

        // Mostrar panel y UI del item
        if (pickupPanel != null)
            pickupPanel.SetActive(true);

        if (pickupUI != null)
            pickupUI.ShowItem(item);

        // Desbloquear habilidad
        if (PlayerAbilities.Instance != null)
            PlayerAbilities.Instance.UnlockAbility(item.ability);
    }

    public void ClosePickup()
    {
        if (pickupPanel != null)
            pickupPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
