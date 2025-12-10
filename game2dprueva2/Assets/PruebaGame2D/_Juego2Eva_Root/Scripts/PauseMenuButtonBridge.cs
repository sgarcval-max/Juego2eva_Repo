using UnityEngine;

public class PauseMenuButtonBridge : MonoBehaviour
{
    public void OpenOptions()
    {
        if (MenuControllerRuntime.Instance != null)
        {
            MenuControllerRuntime.Instance.ShowControlsPanel();
        }
        else
        {
            Debug.LogWarning("No existe MenuControllerRuntime en memoria.");
        }
    }
}
