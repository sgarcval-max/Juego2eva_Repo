using NUnit.Framework.Interfaces;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager instance;

    [Header("UI")]
    public GameObject pickupPanel;
    public ItemPopupUI pickupUI;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void CollectItem(ItemData item)
    {
        Time.timeScale = 0f; // 🔥 PAUSA TOTAL

        pickupPanel.SetActive(true);
        pickupUI.ShowItem(item);

        PlayerAbilities.instance.UnlockAbility(item.ability);
    }

    public void ClosePickup()
    {
        pickupPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
