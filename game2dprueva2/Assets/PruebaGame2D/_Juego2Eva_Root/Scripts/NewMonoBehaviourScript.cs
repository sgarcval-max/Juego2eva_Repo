using UnityEngine;

public class OptionsUIPrefabLoader : MonoBehaviour
{
    private const string prefabPath = "OptionsUI"; // Resources/OptionsUI.prefab
    private static GameObject instance;

    void Awake()
    {
        if (instance != null) return; // ya instanciado por otra escena

        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogWarning("[OptionsUIPrefabLoader] No se encontró prefab en Resources/OptionsUI");
            return;
        }

        GameObject go = Instantiate(prefab);
        go.name = prefab.name;
        DontDestroyOnLoad(go);
        instance = go;
        Debug.Log("[OptionsUIPrefabLoader] OptionsUI instanciado y persistente.");
    }
}
