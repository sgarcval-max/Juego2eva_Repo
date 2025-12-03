using UnityEngine;

public static class ControlBindings
{
    public const string JumpKey = "Jump";
    public const string AttackKey = "Attack";

    // Guardar tecla
    public static void SaveKey(string action, KeyCode key)
    {
        PlayerPrefs.SetInt(action, (int)key);
        PlayerPrefs.Save();
    }

    // Cargar tecla
    public static KeyCode LoadKey(string action, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(action))
            return (KeyCode)PlayerPrefs.GetInt(action);
        else
            return defaultKey;
    }
}
