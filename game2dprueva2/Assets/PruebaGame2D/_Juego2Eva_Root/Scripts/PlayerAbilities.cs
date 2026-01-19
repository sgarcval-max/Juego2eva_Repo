using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities Instance;

    public bool canDoubleJump;
    public bool canChargedFire;
    public bool canMeleeAttack;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UnlockAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.DoubleJump:
                canDoubleJump = true;
                break;

            case AbilityType.ChargedFire:
                canChargedFire = true;
                break;

            case AbilityType.MeleeAttack:
                canMeleeAttack = true;
                break;
        }
    }
}