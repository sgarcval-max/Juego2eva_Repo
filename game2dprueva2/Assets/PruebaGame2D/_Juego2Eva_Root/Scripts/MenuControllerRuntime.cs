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
    public CanvasGroup Menuopciones;
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
            DontDestroyOnLoad(gameObject);

            if (optionsUIPrefab != null)
            {
                optionsUIInstance = Instantiate(optionsUIPrefab, transform);

                // Busca todos los paneles dentro del prefab, incluso si están desactivados
                CanvasGroup[] panels = optionsUIInstance.GetComponentsInChildren<CanvasGroup>(true);
                foreach (var cg in panels)
                {
                    switch (cg.gameObject.name)
                    {
                        case "Menuopciones":
                            Menuopciones = cg;
                            break;
                        case "SoundPanel":
                            soundPanelGroup = cg;
                            break;
                        case "KeyboardPanel":
                            keyboardPanelGroup = cg;
                            break;
                        case "GamepadPanel":
                            gamepadPanelGroup = cg;
                            break;
                        case "KeyboardReassignPanel":
                            keyboardReassignPanel = cg;
                            break;
                        case "GamepadReassignPanel":
                            gamepadReassignPanel = cg;
                            break;
                    }
                }
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        HideAllPanels();
    }

    private void HideAllPanels()
    {
        CanvasGroup[] all = { Menuopciones, soundPanelGroup, keyboardPanelGroup, gamepadPanelGroup, keyboardReassignPanel, gamepadReassignPanel };
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
        if (Menuopciones != null)
            StartCoroutine(FadeOutAndIn(mainMenuGroup, Menuopciones));
        else
            Debug.LogWarning("Menuopciones no asignado.");
    }

    public void ReturnToMainMenuFromOptions()
    {
        if (Menuopciones != null)
            StartCoroutine(FadeOutAndIn(Menuopciones, mainMenuGroup));
    }

    public void ShowSoundPanel() { StartCoroutine(FadeOutAndIn(Menuopciones, soundPanelGroup)); }
    public void ShowKeyboardPanel() { StartCoroutine(FadeOutAndIn(Menuopciones, keyboardPanelGroup)); }
    public void ShowGamepadPanel() { StartCoroutine(FadeOutAndIn(Menuopciones, gamepadPanelGroup)); }
    public void ReturnToControlsPanel(CanvasGroup fromPanel) { StartCoroutine(FadeOutAndIn(fromPanel, Menuopciones)); }

    public void OpenKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardPanelGroup, keyboardReassignPanel)); }
    public void CloseKeyboardReassignPanel() { StartCoroutine(FadeOutAndIn(keyboardReassignPanel, keyboardPanelGroup)); }
    public void OpenGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadPanelGroup, gamepadReassignPanel)); }
    public void CloseGamepadReassignPanel() { StartCoroutine(FadeOutAndIn(gamepadReassignPanel, gamepadPanelGroup)); }

    // -------------------- FADE COROUTINES --------------------

    private IEnumerator FadeInPanel(CanvasGroup cg)
    {
        if (cg == null) yield break;

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
        if (cg == null) yield break;

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