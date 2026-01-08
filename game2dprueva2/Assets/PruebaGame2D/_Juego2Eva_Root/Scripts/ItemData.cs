using UnityEngine;

public enum AbilityType
{
    None,
    DoubleJump,
    Dash,
    Fireball
    // Añade las habilidades que quieras
}

[CreateAssetMenu(menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public AbilityType ability;
}