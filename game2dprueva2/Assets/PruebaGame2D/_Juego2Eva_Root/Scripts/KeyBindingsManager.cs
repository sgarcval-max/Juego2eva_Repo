using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputDeviceType { Keyboard, Gamepad }

public class KeyBindingsManager : MonoBehaviour
{
    public static KeyBindingsManager Instance;

    [Header("Datos")]
    public ActionListSO actionList;

    private Dictionary<string, KeyCode> keyboardBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> gamepadButtonBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, string> gamepadAxisBindings = new Dictionary<string, string>(); // axis names

    public event Action<string, InputDeviceType, object> OnBindingChanged;
    // object puede ser KeyCode o string (axis)

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadBindings();
        Debug.Log("[KeyBindingsManager] Awake - bindings cargados.");
    }

    private void LoadBindings()
    {
        keyboardBindings.Clear();
        gamepadButtonBindings.Clear();
        gamepadAxisBindings.Clear();

        if (actionList == null)
        {
            Debug.LogWarning("[KeyBindingsManager] actionList is NULL.");
            return;
        }

        foreach (var action in actionList.actions)
        {
            string kKey = $"bind_{action.actionName}_Keyboard";
            string gbKey = $"bind_{action.actionName}_GamepadButton";
            string gaKey = $"bind_{action.actionName}_GamepadAxis";

            // keyboard
            if (PlayerPrefs.HasKey(kKey))
            {
                string val = PlayerPrefs.GetString(kKey);
                if (Enum.TryParse(val, out KeyCode parsedK)) keyboardBindings[action.actionName] = parsedK;
                else keyboardBindings[action.actionName] = action.defaultKey;
            }
            else keyboardBindings[action.actionName] = action.defaultKey;

            // gamepad button
            if (PlayerPrefs.HasKey(gbKey))
            {
                string val = PlayerPrefs.GetString(gbKey);
                if (Enum.TryParse(val, out KeyCode parsedG)) gamepadButtonBindings[action.actionName] = parsedG;
                else gamepadButtonBindings[action.actionName] = TryParseGamepadButton(action.defaultGamepadButton);
            }
            else gamepadButtonBindings[action.actionName] = TryParseGamepadButton(action.defaultGamepadButton);

            // gamepad axis
            if (PlayerPrefs.HasKey(gaKey))
                gamepadAxisBindings[action.actionName] = PlayerPrefs.GetString(gaKey);
            else
                gamepadAxisBindings[action.actionName] = action.defaultGamepadAxis ?? "";
        }
    }

    public void SaveBindings()
    {
        foreach (var kv in keyboardBindings)
            PlayerPrefs.SetString($"bind_{kv.Key}_Keyboard", kv.Value.ToString());

        foreach (var kv in gamepadButtonBindings)
            PlayerPrefs.SetString($"bind_{kv.Key}_GamepadButton", kv.Value.ToString());

        foreach (var kv in gamepadAxisBindings)
            PlayerPrefs.SetString($"bind_{kv.Key}_GamepadAxis", kv.Value);

        PlayerPrefs.Save();
        Debug.Log("[KeyBindingsManager] SaveBindings guardado en PlayerPrefs.");
    }

    // getters
    public KeyCode GetBinding(string actionName, InputDeviceType device)
    {
        if (device == InputDeviceType.Keyboard)
        {
            if (keyboardBindings.TryGetValue(actionName, out var k)) return k;
        }
        else
        {
            if (gamepadButtonBindings.TryGetValue(actionName, out var g)) return g;
        }
        return KeyCode.None;
    }

    public string GetGamepadAxisBinding(string actionName)
    {
        if (gamepadAxisBindings.TryGetValue(actionName, out var a)) return a;
        return "";
    }

    // rebinders
    public void Rebind(string actionName, KeyCode newKey, InputDeviceType device)
    {
        if (device == InputDeviceType.Keyboard) keyboardBindings[actionName] = newKey;
        else gamepadButtonBindings[actionName] = newKey;

        SaveBindings();
        Debug.Log($"[KeyBindingsManager] Rebind -> {actionName} ({device}) = {newKey}");

        // Notificar
        if (device == InputDeviceType.Keyboard) KeyboardVisualizer.Instance?.UpdateAction(actionName);
        else GamepadVisualizer.Instance?.UpdateAction(actionName);

        OnBindingChanged?.Invoke(actionName, device, newKey);
    }

    public void RebindGamepadAxis(string actionName, string axisName)
    {
        gamepadAxisBindings[actionName] = axisName ?? "";
        SaveBindings();
        Debug.Log($"[KeyBindingsManager] RebindAxis -> {actionName} = {axisName}");

        GamepadVisualizer.Instance?.UpdateAction(actionName);
        OnBindingChanged?.Invoke(actionName, InputDeviceType.Gamepad, axisName);
    }

    // utilidad
    public KeyCode GetEffectiveBinding(string actionName)
    {
        if (IsGamepadConnected())
        {
            var g = GetBinding(actionName, InputDeviceType.Gamepad);
            if (g != KeyCode.None) return g;
            var axis = GetGamepadAxisBinding(actionName);
            if (!string.IsNullOrEmpty(axis)) return KeyCode.None; // indicates axis present
        }
        return GetBinding(actionName, InputDeviceType.Keyboard);
    }

    private KeyCode TryParseGamepadButton(string defaultGamepadButton)
    {
        if (string.IsNullOrEmpty(defaultGamepadButton)) return KeyCode.None;
        if (Enum.TryParse(defaultGamepadButton, out KeyCode parsed)) return parsed;
        return KeyCode.None;
    }

    public bool IsGamepadConnected()
    {
        var names = Input.GetJoystickNames();
        for (int i = 0; i < names.Length; i++)
            if (!string.IsNullOrEmpty(names[i])) return true;
        return false;
    }

    // reset opcional
    public void ResetToDefaults(bool save = true)
    {
        foreach (var action in actionList.actions)
        {
            keyboardBindings[action.actionName] = action.defaultKey;
            gamepadButtonBindings[action.actionName] = TryParseGamepadButton(action.defaultGamepadButton);
            gamepadAxisBindings[action.actionName] = action.defaultGamepadAxis ?? "";
        }
        if (save) SaveBindings();
        Debug.Log("[KeyBindingsManager] Reset a defaults.");
    }
}