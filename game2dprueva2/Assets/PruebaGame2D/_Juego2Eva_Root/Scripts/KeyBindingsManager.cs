using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputDeviceType { Keyboard, Gamepad }

// Nota: ActionListSO y ActionData deben existir en tu proyecto.
// ActionData debe exponer: string actionName; KeyCode defaultKey; string defaultGamepadButton;
public class KeyBindingsManager : MonoBehaviour
{
    public static KeyBindingsManager Instance;

    [Header("Data")]
    public ActionListSO actionList;

    // bindings por dispositivo
    private Dictionary<string, KeyCode> keyboardBindings = new Dictionary<string, KeyCode>();
    private Dictionary<string, KeyCode> gamepadBindings = new Dictionary<string, KeyCode>();

    // Evento opcional: permite que otras clases se suscriban a cambios
    public event Action<string, InputDeviceType, KeyCode> OnBindingChanged;

    private void Awake()
    {
        // singleton clásico
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cargar desde PlayerPrefs
        LoadBindings();

        Debug.Log("[KeyBindingsManager] Awake - bindings cargados.");
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

            // Keyboard
            if (PlayerPrefs.HasKey(kKey))
            {
                string val = PlayerPrefs.GetString(kKey);
                if (Enum.TryParse(val, out KeyCode parsedK))
                    keyboardBindings[action.actionName] = parsedK;
                else
                {
                    Debug.LogWarning($"[KeyBindingsManager] No se pudo parsear '{val}' para {kKey}. Uso default.");
                    keyboardBindings[action.actionName] = action.defaultKey;
                }
            }
            else
                keyboardBindings[action.actionName] = action.defaultKey;

            // Gamepad
            if (PlayerPrefs.HasKey(gKey))
            {
                string val = PlayerPrefs.GetString(gKey);
                if (Enum.TryParse(val, out KeyCode parsedG))
                    gamepadBindings[action.actionName] = parsedG;
                else
                {
                    Debug.LogWarning($"[KeyBindingsManager] No se pudo parsear '{val}' para {gKey}. Uso default.");
                    gamepadBindings[action.actionName] = TryParseGamepadDefault(action.defaultGamepadButton);
                }
            }
            else
                gamepadBindings[action.actionName] = TryParseGamepadDefault(action.defaultGamepadButton);
        }
    }

    // Guarda todo en PlayerPrefs
    public void SaveBindings()
    {
        foreach (var kv in keyboardBindings)
            PlayerPrefs.SetString($"bind_{kv.Key}_Keyboard", kv.Value.ToString());

        foreach (var kv in gamepadBindings)
            PlayerPrefs.SetString($"bind_{kv.Key}_Gamepad", kv.Value.ToString());

        PlayerPrefs.Save();
        Debug.Log("[KeyBindingsManager] SaveBindings: guardado en PlayerPrefs.");
    }

    // Get binding para un dispositivo concreto
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

    // Rebind: ahora guarda + notifica + dispara evento
    public void Rebind(string actionName, KeyCode newKey, InputDeviceType device)
    {
        if (device == InputDeviceType.Keyboard)
            keyboardBindings[actionName] = newKey;
        else
            gamepadBindings[actionName] = newKey;

        SaveBindings();

        Debug.Log($"[KeyBindingsManager] Rebind -> {actionName} ({device}) = {newKey}");

        // Notificar a visualizadores concretos (si existen)
        if (device == InputDeviceType.Keyboard)
            KeyboardVisualizer.Instance?.UpdateAction(actionName);
        else
            GamepadVisualizer.Instance?.UpdateAction(actionName);

        // Disparar evento para quien quiera escuchar
        OnBindingChanged?.Invoke(actionName, device, newKey);
    }

    // Helper: prioridad gamepad si está conectado y tiene binding
    public KeyCode GetEffectiveBinding(string actionName)
    {
        bool gamepadConnected = IsGamepadConnected();
        if (gamepadConnected)
        {
            var g = GetBinding(actionName, InputDeviceType.Gamepad);
            if (g != KeyCode.None) return g;
        }
        return GetBinding(actionName, InputDeviceType.Keyboard);
    }

    // Reset a defaults (útil para debug / opciones)
    public void ResetToDefaults(bool save = true)
    {
        foreach (var action in actionList.actions)
        {
            keyboardBindings[action.actionName] = action.defaultKey;
            if (!string.IsNullOrEmpty(action.defaultGamepadButton) && Enum.TryParse(action.defaultGamepadButton, out KeyCode parsed))
                gamepadBindings[action.actionName] = parsed;
            else
                gamepadBindings[action.actionName] = KeyCode.None;
        }

        if (save) SaveBindings();
        Debug.Log("[KeyBindingsManager] Reset to defaults.");
    }

    private KeyCode TryParseGamepadDefault(string defaultGamepadButton)
    {
        if (string.IsNullOrEmpty(defaultGamepadButton)) return KeyCode.None;
        if (Enum.TryParse(defaultGamepadButton, out KeyCode parsed)) return parsed;
        return KeyCode.None;
    }

    private bool IsGamepadConnected()
    {
        string[] names = Input.GetJoystickNames();
        for (int i = 0; i < names.Length; i++)
            if (!string.IsNullOrEmpty(names[i])) return true;
        return false;
    }
}