using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardVisualizer : MonoBehaviour
{
    public static KeyboardVisualizer Instance;

    [System.Serializable]
    public class KeyHotspot
    {
        public string keyName; // ej "A", "Space", "Q"
        public RectTransform hotspotTransform; // arrastra el RectTransform hijo aquí
        public GameObject arrowContainer; // arrastra el ArrowContainer (hijo) aquí
        public TMP_Text label; // arrastra el TMP_Text (Label) aquí
    }

    [Header("Hotspots (map each visible key)")]
    public List<KeyHotspot> hotspots = new List<KeyHotspot>();

    private Dictionary<string, KeyHotspot> hotspotMap = new Dictionary<string, KeyHotspot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        hotspotMap.Clear();
        foreach (var hs in hotspots)
        {
            if (hs != null && !string.IsNullOrEmpty(hs.keyName))
                hotspotMap[hs.keyName.ToUpper()] = hs;
        }
    }

    // Call this after bindings change
    public void UpdateAllKeys()
    {
        // hide all first
        foreach (var hs in hotspots)
        {
            if (hs.arrowContainer != null) hs.arrowContainer.SetActive(false);
            if (hs.label != null) hs.label.text = "";
        }

        if (KeyBindingsManager.Instance == null) return;
        var actionList = KeyBindingsManager.Instance.actionList;
        if (actionList == null) return;

        foreach (var action in actionList.actions)
        {
            string actionName = action.actionName;
            KeyCode boundKey = KeyBindingsManager.Instance.GetBinding(actionName, InputDeviceType.Keyboard);
            if (boundKey == KeyCode.None) continue;
            string keyStr = boundKey.ToString().ToUpper();

            if (hotspotMap.TryGetValue(keyStr, out KeyHotspot hs))
            {
                if (hs.arrowContainer != null) hs.arrowContainer.SetActive(true);
                if (hs.label != null) hs.label.text = actionName;
            }
            else
            {
                Debug.LogWarning($"[KeyboardVisualizer] No hotspot found for key '{keyStr}' (action {actionName}). Add hotspot named '{keyStr}'.");
            }
        }
    }

    public void UpdateAction(string actionName)
    {
        foreach (var hs in hotspots)
        {
            if (hs.label != null && hs.label.text == actionName)
            {
                hs.label.text = "";
                if (hs.arrowContainer != null) hs.arrowContainer.SetActive(false);
            }
        }

        KeyCode boundKey = KeyBindingsManager.Instance.GetBinding(actionName, InputDeviceType.Keyboard);
        if (boundKey == KeyCode.None) return;
        string keyStr = boundKey.ToString().ToUpper();

        if (hotspotMap.TryGetValue(keyStr, out KeyHotspot hs2))
        {
            if (hs2.arrowContainer != null) hs2.arrowContainer.SetActive(true);
            if (hs2.label != null) hs2.label.text = actionName;
        }
    }

    void OnEnable()
    {
        UpdateAllKeys();
    }
}
