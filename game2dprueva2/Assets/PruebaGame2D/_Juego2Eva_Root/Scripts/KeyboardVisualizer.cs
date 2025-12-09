using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardVisualizer : MonoBehaviour
{
    public static KeyboardVisualizer Instance;

    [System.Serializable]
    public class KeyHotspot
    {
        public string keyName; // ej "A", "Space"
        public RectTransform hotspotTransform; // el objeto vacío que posicionaste sobre la tecla
        public GameObject arrowContainer; // flecha + label (desactivado por defecto)
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
            KeyCode bound = KeyBindingsManager.Instance.GetBinding(actionName, InputDeviceType.Keyboard);
            if (bound == KeyCode.None) continue;

            string keyStr = bound.ToString().ToUpper();
            if (hotspotMap.TryGetValue(keyStr, out KeyHotspot hs))
            {
                if (hs.arrowContainer != null) hs.arrowContainer.SetActive(true);
                if (hs.label != null) hs.label.text = actionName;
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

        KeyCode bound = KeyBindingsManager.Instance.GetBinding(actionName, InputDeviceType.Keyboard);
        if (bound == KeyCode.None) return;
        string keyStr = bound.ToString().ToUpper();
        if (hotspotMap.TryGetValue(keyStr, out KeyHotspot hs2))
        {
            if (hs2.arrowContainer != null) hs2.arrowContainer.SetActive(true);
            if (hs2.label != null) hs2.label.text = actionName;
        }
    }
}
