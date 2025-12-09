using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GamepadVisualizer : MonoBehaviour
{
    public static GamepadVisualizer Instance;

    [System.Serializable]
    public class KeyHotspot
    {
        public string keyName; // e.g. "JOYSTICKBUTTON0" or "LEFTSTICK"
        public RectTransform hotspotTransform;
        public GameObject arrowContainer;
        public TMP_Text label;
    }

    public List<KeyHotspot> hotspots = new List<KeyHotspot>();
    private Dictionary<string, KeyHotspot> hotspotMap = new Dictionary<string, KeyHotspot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        hotspotMap.Clear();
        foreach (var hs in hotspots)
            if (hs != null && !string.IsNullOrEmpty(hs.keyName))
                hotspotMap[hs.keyName.ToUpper()] = hs;
    }

    void OnEnable() => UpdateAllKeys();

    public void UpdateAllKeys()
    {
        foreach (var hs in hotspots)
        {
            if (hs.arrowContainer != null) hs.arrowContainer.SetActive(false);
            if (hs.label != null) hs.label.text = "";
        }

        if (KeyBindingsManager.Instance == null) return;
        var list = KeyBindingsManager.Instance.actionList;
        if (list == null) return;

        foreach (var action in list.actions)
        {
            string actionName = action.actionName;

            // Primero mirar si tiene axis
            string axis = KeyBindingsManager.Instance.GetGamepadAxisBinding(actionName);
            if (!string.IsNullOrEmpty(axis))
            {
                // map axis to hotspot key, e.g. "LeftStickX" -> "LEFTSTICK"
                string hk = MapAxisToHotspotKey(axis);
                if (hotspotMap.TryGetValue(hk, out KeyHotspot hsAxis))
                {
                    if (hsAxis.arrowContainer != null) hsAxis.arrowContainer.SetActive(true);
                    if (hsAxis.label != null) hsAxis.label.text = actionName;
                    continue;
                }
            }

            // Else, check button
            KeyCode bound = KeyBindingsManager.Instance.GetBinding(actionName, InputDeviceType.Gamepad);
            if (bound == KeyCode.None) continue;
            string keyStr = bound.ToString().ToUpper();
            if (hotspotMap.TryGetValue(keyStr, out KeyHotspot hsBtn))
            {
                if (hsBtn.arrowContainer != null) hsBtn.arrowContainer.SetActive(true);
                if (hsBtn.label != null) hsBtn.label.text = actionName;
            }
        }
    }

    public void UpdateAction(string actionName)
    {
        // limpiar anterior
        foreach (var hs in hotspots)
            if (hs.label != null && hs.label.text == actionName)
            {
                hs.label.text = "";
                if (hs.arrowContainer != null) hs.arrowContainer.SetActive(false);
            }

        // axis primero
        string axis = KeyBindingsManager.Instance.GetGamepadAxisBinding(actionName);
        if (!string.IsNullOrEmpty(axis))
        {
            string hk = MapAxisToHotspotKey(axis);
            if (hotspotMap.TryGetValue(hk, out KeyHotspot hsAxis))
            {
                if (hsAxis.arrowContainer != null) hsAxis.arrowContainer.SetActive(true);
                if (hsAxis.label != null) hsAxis.label.text = actionName;
                return;
            }
        }

        // botón
        KeyCode bound = KeyBindingsManager.Instance.GetBinding(actionName, InputDeviceType.Gamepad);
        if (bound == KeyCode.None) return;
        string keyStr = bound.ToString().ToUpper();
        if (hotspotMap.TryGetValue(keyStr, out KeyHotspot hsBtn))
        {
            if (hsBtn.arrowContainer != null) hsBtn.arrowContainer.SetActive(true);
            if (hsBtn.label != null) hsBtn.label.text = actionName;
        }
    }

    // Simple mapping: LeftStickX/LeftStickY -> LEFTSTICK, RightStick* -> RIGHTSTICK, DPad* -> DPAD
    private string MapAxisToHotspotKey(string axis)
    {
        axis = axis.ToUpper();
        if (axis.Contains("LEFTSTICK")) return "LEFTSTICK";
        if (axis.Contains("RIGHTSTICK")) return "RIGHTSTICK";
        if (axis.Contains("DPAD")) return "DPAD";
        return axis; // fallback
    }
}
