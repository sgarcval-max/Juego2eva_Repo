using System.Collections;
using UnityEngine;

public class MenuControllerRuntime : MonoBehaviour
{
    public static MenuControllerRuntime Instance;

    [Header("Prefab Options UI")]
    public GameObject optionsUIPrefab; // Arrastra tu prefab OptionsUI aquí
    private GameObject optionsUIInstance;

    [Header("Scene Main Menu")]
    public CanvasGroup mainMenuGroup; // Se oculta al abrir Options

    [Header("Options UI Prefab Panels")]
    public CanvasGroup controlsPanelGroup;
    public CanvasGroup soundPanelGroup;
    public CanvasGroup keyboardPanelGroup;
    public CanvasGroup gamepadPanelGroup;
    public CanvasGroup keyboardReassignPanel;
    public CanvasGroup gamepadReassignPanel;

    [Header("Transition")]
    public float transitionDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // El MenuControllerRuntime no se destruye
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Instanciar OptionsUI si no existe
        if (optionsUIInstance == null && optionsUIPrefab != null)
        {
            optionsUIInstance = Instantiate(optionsUIPrefab);
            DontDestroyOnLoad(optionsUIInstance);

            // Asignar referencias de UIReferenceHolder si existe
            UIReferenceHolder uiRef = optionsUIInstance.GetComponent<UIReferenceHolder>();
            if (uiRef != null)
            {
                controlsPanelGroup = uiRef.controlsPanelGroup;
                soundPanelGroup = uiRef.soundPanelGroup;
                keyboardPanelGroup = uiRef.keyboardPanelGroup;
                gamepadPanelGroup = uiRef.gamepadPanelGroup;
                keyboardReassignPanel = uiRef.keyboardReassignPanel;
                gamepadReassignPanel = uiRef.gamepadReassignPanel;
            }
        }

        HideAllPanels();
    }

    private void HideAllPanels()
    {
        CanvasGroup[] all = { controlsPanelGroup, soundPanelGroup, keyboardPanelGroup, gamepadPanelGroup, keyboardReassignPanel, gamepadReassignPanel };
        foreach (var cg in all)
        {
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }

    // -------------------- PANEL METHODS --------------------

    public void ShowControlsPanel()
    {
        StartCoroutine(FadeOutAndIn(mainMenuGroup, controlsPanelGroup));
    }

    public void ReturnToMainMenuFromOptions()
    {
        StartCoroutine(FadeOutAndIn(controlsPanelGroup, mainMenuGroup));
    }

    public void ShowSoundPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, soundPanelGroup)); }
    public void ShowKeyboardPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, keyboardPanelGroup)); }
    public void ShowGamepadPanel() { StartCoroutine(FadeOutAndIn(controlsPanelGroup, gamepadPanelGroup)); }
    public void ReturnToControlsPanel(CanvasGroup fromPanel) { StartCoroutine(FadeOutAndIn(fromPanel, controlsPanelGroup)); }

    public void OpenKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardPanelGroup, keyboardReassignPanel)); }
    public void CloseKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardReassignPanel, keyboardPanelGroup)); }
    public void OpenGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadPanelGroup, gamepadReassignPanel)); }
    public void CloseGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadReassignPanel, gamepadPanelGroup)); }

    // -------------------- FADE COROUTINES --------------------

    private IEnumerator FadeInPanel(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        float t = 0f;
        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / transitionDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    private IEnumerator FadeOutPanel(CanvasGroup cg)
    {
        float t = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / transitionDuration);
            yield return null;
        }
        cg.alpha = 0f;
    }

    private IEnumerator FadeOutAndIn(CanvasGroup fadeOutGroup, CanvasGroup fadeInGroup)
    {
        yield return FadeOutPanel(fadeOutGroup);
        yield return FadeInPanel(fadeInGroup);
    }
}