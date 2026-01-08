using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities instance;

    public bool canDash;
    public bool canDoubleJump;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void UnlockAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Dash:
                canDash = true;
                break;

            case AbilityType.DoubleJump:
                canDoubleJump = true;
                break;
        }
    }
}