using System.Collections.Generic;
using UnityEngine;

public class KeyBindingsManager : MonoBehaviour
{
    public static KeyBindingsManager Instance;

    [Header("Assignment (optional)")]
    public ActionListSO actionList; // asigna aquí si quieres en el prefab

    private Dictionary<string, KeyCode> bindings = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        // Si ya existe una instancia viva, destruimos el duplicado
        if (Instance != null)
        {
            Debug.Log("[KeyBindingsManager] Duplicate instance found. Destroying new one: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        // Intentar encontrar una instancia (por si el prefab fue colocado en escena)
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[KeyBindingsManager] Awake -> Instance set and DontDestroyOnLoad on: " + gameObject.name);

        // Si actionList no está asignado (ej: arrancado desde otra escena), intentamos cargar datos desde Resources
        if (actionList == null)
        {
            ActionListSO so = Resources.Load<ActionListSO>("ActionListSO"); // nombre del asset dentro de Resources
            if (so != null)
            {
                actionList = so;
                Debug.Log("[KeyBindingsManager] Loaded ActionListSO from Resources: " + so.name);
            }
            else
            {
                Debug.LogWarning("[KeyBindingsManager] No ActionListSO assigned and none found in Resources (nombre esperado: 'ActionListSO').");
            }
        }

        LoadBindings();
    }

    // Fallback: si no existe ninguna instancia en ejecución, intenta instanciar el prefab automáticamente.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance != null) return;

        // Buscar en escena
        KeyBindingsManager existing = Object.FindObjectOfType<KeyBindingsManager>();
        if (existing != null)
        {
            Instance = existing;
            DontDestroyOnLoad(existing.gameObject);
            Debug.Log("[KeyBindingsManager] Found existing instance in scene at runtime. Making persistent.");
            return;
        }

        // Intentar cargar prefab de Resources
        GameObject prefab = Resources.Load<GameObject>("KeyBindingsManager"); // Resources/KeyBindingsManager.prefab
        if (prefab != null)
        {
            GameObject go = Instantiate(prefab);
            go.name = prefab.name;
            Instance = go.GetComponent<KeyBindingsManager>();
            DontDestroyOnLoad(go);
            Debug.Log("[KeyBindingsManager] Instantiated KeyBindingsManager prefab from Resources at startup.");
            return;
        }

        Debug.Log("[KeyBindingsManager] No instance and no prefab in Resources. You should add KeyBindingsManager prefab to Resources.");
    }

    private void LoadBindings()
    {
        bindings.Clear();

        if (actionList == null)
        {
            Debug.LogWarning("[KeyBindingsManager] actionList is null in LoadBindings().");
            return;
        }

        foreach (var action in actionList.actions)
        {
            string keyName = "key_" + action.actionName;

            if (PlayerPrefs.HasKey(keyName))
                bindings[action.actionName] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyName));
            else
                bindings[action.actionName] = action.defaultKey;
        }
    }

    public void SaveBindings()
    {
        foreach (var b in bindings)
        {
            PlayerPrefs.SetString("key_" + b.Key, b.Value.ToString());
        }

        PlayerPrefs.Save();
    }

    public KeyCode GetBinding(string action)
    {
        if (bindings.TryGetValue(action, out KeyCode k)) return k;
        return KeyCode.None;
    }

    public void Rebind(string action, KeyCode key)
    {
        bindings[action] = key;
        SaveBindings();
    }
}