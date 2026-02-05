using UnityEngine;

public enum AbilityType
{
    DoubleJump,
    ChargedFire,
    MeleeAttack
}

public class ItemData : MonoBehaviour
{
    public AbilityType ability;

    [Header("UI Info")]
    public Sprite icon;
    public string itemName;
    [TextArea]
    public string description;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockAbility();

            // Mostrar popup de UI
            ItemPopupUI popup = FindObjectOfType<ItemPopupUI>();
            if (popup != null)
                popup.ShowItem(this);

            Destroy(gameObject);
        }
    }

    void UnlockAbility()
    {
        switch (ability)
        {
            case AbilityType.DoubleJump:
                AbilityManager.Instance.doubleJumpUnlocked = true;

                // 🔥 UI icono 1
                if (AbilityUIManager.Instance != null)
                    AbilityUIManager.Instance.UnlockAbility(0); // <-- actualizado
                break;

            case AbilityType.ChargedFire:
                AbilityManager.Instance.chargedFireUnlocked = true;

                // 🔥 UI icono 2
                if (AbilityUIManager.Instance != null)
                    AbilityUIManager.Instance.UnlockAbility(1); // <-- actualizado
                break;

            case AbilityType.MeleeAttack:
                AbilityManager.Instance.meleeAttackUnlocked = true;

                // 🔥 UI icono 3
                if (AbilityUIManager.Instance != null)
                    AbilityUIManager.Instance.UnlockAbility(2); // <-- actualizado
                break;
        }

        Debug.Log("Habilidad desbloqueada: " + ability);
    }
}