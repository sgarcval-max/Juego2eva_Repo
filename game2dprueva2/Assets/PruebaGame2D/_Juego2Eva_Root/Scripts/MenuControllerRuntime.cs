using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuControllerRuntime : MonoBehaviour
{
    public static MenuControllerRuntime Instance;

    [Tooltip("Ruta dentro de Resources del prefab Options UI sin extensión (ej: 'OptionsUI')")]
    public string optionsUIPrefabResourcePath = "OptionsUI";

    private GameObject optionsUIInstance;
    private CanvasGroup controlsPanelGroup;
    private CanvasGroup mainMenuGroupInPrefab; // si tu prefab tiene un panel principal interno (opcional)

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        // Intenta instanciar el prefab en Awake (para que esté disponible siempre)
        EnsureOptionsUI();
    }

    // Llama a esto si quieres forzar la creación (por si arrancas desde otra escena)
    public void EnsureOptionsUI()
    {
        if (optionsUIInstance != null) return;

        GameObject prefab = Resources.Load<GameObject>(optionsUIPrefabResourcePath);
        if (prefab == null)
        {
            Debug.LogWarning($"[MenuControllerRuntime] No se encontró prefab Resources/{optionsUIPrefabResourcePath}");
            return;
        }

        optionsUIInstance = Instantiate(prefab);
        optionsUIInstance.name = prefab.name;
        DontDestroyOnLoad(optionsUIInstance);

        CacheReferences();
        WireInternalButtons();
        // por defecto ocultamos el panel de opciones
        if (controlsPanelGroup != null) { controlsPanelGroup.alpha = 0f; controlsPanelGroup.interactable = false; controlsPanelGroup.blocksRaycasts = false; }
    }

    private void CacheReferences()
    {
        // Ajusta estas rutas si tus objetos se llaman distinto dentro del prefab
        var root = optionsUIInstance.transform;

        var cp = root.Find("ControlsPanelGroup");
        if (cp != null) controlsPanelGroup = cp.GetComponent<CanvasGroup>();

        var mm = root.Find("MainOptionsContent");
        if (mm != null) mainMenuGroupInPrefab = mm.GetComponent<CanvasGroup>();
    }

    private void WireInternalButtons()
    {
        // ejemplo: BackButton dentro del prefab que debe cerrar el panel options
        var back = optionsUIInstance.transform.Find("ControlsPanelGroup/BackButton");
        if (back != null)
        {
            var b = back.GetComponent<Button>();
            if (b != null)
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() => ShowMainMenuFromOptions());
            }
        }
    }

    // Abre el panel de opciones (fade in)
    public void ShowControlsPanel(float duration = 0.45f)
    {
        EnsureOptionsUI();
        if (controlsPanelGroup == null) { Debug.LogWarning("[MenuControllerRuntime] controlsPanelGroup no asignado"); return; }
        StartCoroutine(FadeCanvasGroup(controlsPanelGroup, 0f, 1f, duration, true));
    }

    // Cierra el panel de opciones y vuelve al menu principal de la escena que lo llamó (si aplica)
    public void ShowMainMenuFromOptions(float duration = 0.45f)
    {
        if (controlsPanelGroup == null) return;
        StartCoroutine(FadeCanvasGroup(controlsPanelGroup, 1f, 0f, duration, false));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float dur, bool enableOnEnd)
    {
        float t = 0f;
        cg.alpha = from;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        cg.alpha = to;
        cg.interactable = enableOnEnd;
        cg.blocksRaycasts = enableOnEnd;
    }
}
