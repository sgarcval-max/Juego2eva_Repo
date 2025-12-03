using UnityEngine;

public static class ControlsManager
{
    // Guarda la tecla elegida para una acción
    public static void SaveKey(string action, KeyCode key)
    {
        PlayerPrefs.SetInt(action, (int)key);
        PlayerPrefs.Save();
    }

    // Devuelve la tecla guardada o la por defecto si no hay
    public static KeyCode LoadKey(string action, KeyCode defaultKey)
    {
        if (PlayerPrefs.HasKey(action))
            return (KeyCode)PlayerPrefs.GetInt(action);
        else
            return defaultKey;
    }
}