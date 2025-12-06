using System.Collections.Generic;
using UnityEngine;

public enum InputDeviceType { Keyboard, Gamepad }

public class KeyBindingsManager : MonoBehaviour
{
    public static KeyBindingsManager Instance;

    public ActionListSO actionList;

    private Dictionary<string, KeyCode> keyboardBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> gamepadBindings = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadBindings();
    }

    private void LoadBindings()
    {
        keyboardBindings.Clear();
        gamepadBindings.Clear();

        if (actionList == null)
        {
            Debug.LogWarning("[KeyBindingsManager] actionList is NULL.");
            return;
        }

        foreach (var action in actionList.actions)
        {
            string kKey = $"bind_{action.actionName}_Keyboard";
            string gKey = $"bind_{action.actionName}_Gamepad";

            // Keyboard: stored as string name of KeyCode
            if (PlayerPrefs.HasKey(kKey))
                keyboardBindings[action.actionName] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(kKey));
            else
                keyboardBindings[action.actionName] = action.defaultKey;

            // Gamepad: defaultGamepad can be stored as KeyCode joystick enum or a string in ActionData
            if (PlayerPrefs.HasKey(gKey))
                gamepadBindings[action.actionName] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(gKey));
            else
            {
                // if ActionData contains a default joystick key string, try parse, else set to KeyCode.None
                if (!string.IsNullOrEmpty(action.defaultGamepadButton))
                {
                    try
                    {
                        gamepadBindings[action.actionName] = (KeyCode)System.Enum.Parse(typeof(KeyCode), action.defaultGamepadButton);
                    }
                    catch { gamepadBindings[action.actionName] = KeyCode.None; }
                }
                else gamepadBindings[action.actionName] = KeyCode.None;
            }
        }
    }

    public void SaveBindings()
    {
        foreach (var kv in keyboardBindings)
            PlayerPrefs.SetString($"bind_{kv.Key}_Keyboard", kv.Value.ToString());

        foreach (var kv in gamepadBindings)
            PlayerPrefs.SetString($"bind_{kv.Key}_Gamepad", kv.Value.ToString());

        PlayerPrefs.Save();
    }

    // Get binding for a device
    public KeyCode GetBinding(string actionName, InputDeviceType device)
    {
        if (device == InputDeviceType.Keyboard)
        {
            if (keyboardBindings.TryGetValue(actionName, out var k)) return k;
        }
        else
        {
            if (gamepadBindings.TryGetValue(actionName, out var g)) return g;
        }
        return KeyCode.None;
    }

    // Rebind for a device
    public void Rebind(string actionName, KeyCode newKey, InputDeviceType device)
    {
        if (device == InputDeviceType.Keyboard)
            keyboardBindings[actionName] = newKey;
        else
            gamepadBindings[actionName] = newKey;

        SaveBindings();
        Debug.Log($"[KeyBindingsManager] Saved {actionName} for {device} = {newKey}");
    }

    // Helper: return best effective binding (prefer gamepad if connected)
    public KeyCode GetEffectiveBinding(string actionName)
    {
        // Aquí decides prioridad: por ejemplo, si hay un gamepad conectado usamos ese; si no, teclado.
        bool gamepadConnected = IsGamepadConnected();
        if (gamepadConnected)
        {
            var g = GetBinding(actionName, InputDeviceType.Gamepad);
            if (g != KeyCode.None) return g;
        }
        // fallback keyboard
        return GetBinding(actionName, InputDeviceType.Keyboard);
    }

    private bool IsGamepadConnected()
    {
        // Simple detection: si hay joystick names no vacíos
        return Input.GetJoystickNames().Length > 0 && !string.IsNullOrEmpty(Input.GetJoystickNames()[0]);
    }
}