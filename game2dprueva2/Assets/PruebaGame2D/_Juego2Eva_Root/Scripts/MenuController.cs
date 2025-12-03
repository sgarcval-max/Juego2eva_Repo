using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Header("UI refs")]
    public CanvasGroup fadePanel;           // Fade panel canvasgroup (negro)
    public Animator titleAnimator;          // Animator del TitlePanel
    public RectTransform titleRect;         // RectTransform del TitlePanel
    public Animator[] buttonAnimators;      // Animators de cada botón (orden: Play, Options, Exit)
    public GameObject firstSelected;        // boton seleccionado inicialmente (Play)

    [Header("Timings")]
    public float initialFadeDuration = 1f;
    public float titleAppearDelay = 0.2f;
    public float betweenButtonsDelay = 0.12f;

    [Header("Press Space Message (Blink)")]
    public GameObject pressSpaceText;
    public float blinkSpeed = 0.8f;
    private Coroutine blinkCoroutine = null;

    [Header("Controls Panel & Fading")]
    public CanvasGroup mainMenuGroup;      // Contiene botones principales
    public CanvasGroup controlsPanelGroup; // Contiene botones de opciones
    public float transitionDuration = 0.5f;

    [Header("Submenu Panels")]
    public CanvasGroup soundPanelGroup;
    public CanvasGroup keyboardPanelGroup;
    public CanvasGroup gamepadPanelGroup;

    private bool inputEnabled = false;

    void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.alpha = 1f;
            fadePanel.blocksRaycasts = true;
        }

        foreach (var b in buttonAnimators)
            b.gameObject.SetActive(true);

        if (pressSpaceText != null)
            pressSpaceText.SetActive(false);

        StartCoroutine(StartupSequence());
    }

    IEnumerator StartupSequence()
    {
        float t = 0f;
        while (t < initialFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            if (fadePanel != null) fadePanel.alpha = 1f - (t / initialFadeDuration);
            yield return null;
        }
        if (fadePanel != null) { fadePanel.alpha = 0f; fadePanel.blocksRaycasts = false; }

        yield return new WaitForSecondsRealtime(titleAppearDelay);
        titleAnimator.SetTrigger("Appear");

        yield return new WaitForSecondsRealtime(0.9f);
        inputEnabled = true;

        if (pressSpaceText != null)
        {
            pressSpaceText.SetActive(true);
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkTextCoroutine());
        }

        if (firstSelected != null) EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space))
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
            if (pressSpaceText != null)
                pressSpaceText.SetActive(false);

            inputEnabled = false;
            StartCoroutine(PlayTitleToMenu());
        }
    }

    IEnumerator PlayTitleToMenu()
    {
        titleAnimator.SetTrigger("Disappear");
        yield return new WaitForSecondsRealtime(0.6f);

        titleAnimator.SetTrigger("ToSmall");
        yield return new WaitForSecondsRealtime(0.6f);

        titleAnimator.SetTrigger("Appear");
        yield return new WaitForSecondsRealtime(0.5f);

        for (int i = 0; i < buttonAnimators.Length; i++)
        {
            buttonAnimators[i].SetTrigger("Show");
            yield return new WaitForSecondsRealtime(betweenButtonsDelay);
        }

        if (firstSelected != null) EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void OnPlayPressed()
    {
        StartCoroutine(DoSceneLoad("IntroCutscene1"));
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }

    IEnumerator DoSceneLoad(string IntroCutscene1)
    {
        if (fadePanel != null) fadePanel.blocksRaycasts = true;
        float dur = 0.9f;
        float t = 0;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            if (fadePanel != null) fadePanel.alpha = t / dur;
            yield return null;
        }
        if (!string.IsNullOrEmpty(IntroCutscene1)) UnityEngine.SceneManagement.SceneManager.LoadScene(IntroCutscene1);
    }

    private IEnumerator BlinkTextCoroutine()
    {
        while (true)
        {
            if (pressSpaceText != null)
                pressSpaceText.SetActive(!pressSpaceText.activeSelf);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    // -------------------- SUBMENU METHODS --------------------
    public void ShowControlsPanel()
    {
        StartCoroutine(FadeOutAndIn(mainMenuGroup, controlsPanelGroup));
    }

    public void ShowSoundPanel()
    {
        StartCoroutine(FadeOutAndIn(controlsPanelGroup, soundPanelGroup));
    }

    public void ShowKeyboardPanel()
    {
        StartCoroutine(FadeOutAndIn(controlsPanelGroup, keyboardPanelGroup));
    }

    public void ShowGamepadPanel()
    {
        StartCoroutine(FadeOutAndIn(controlsPanelGroup, gamepadPanelGroup));
    }

    public void ReturnToOptionsPanelFromSubmenu(CanvasGroup fromPanel)
    {
        StartCoroutine(FadeOutAndIn(fromPanel, controlsPanelGroup));
    }

    private IEnumerator FadeOutAndIn(CanvasGroup fadeOutGroup, CanvasGroup fadeInGroup)
    {
        float duration = transitionDuration;
        float t = 0f;

        fadeOutGroup.interactable = false;
        fadeOutGroup.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeOutGroup.alpha = 1f - (t / duration);
            yield return null;
        }
        fadeOutGroup.alpha = 0f;

        fadeInGroup.alpha = 0;
        fadeInGroup.interactable = true;
        fadeInGroup.blocksRaycasts = true;

        t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeInGroup.alpha = t / duration;
            yield return null;
        }
        fadeInGroup.alpha = 1f;
    }

    public void ShowMainMenu()
    {
        StartCoroutine(FadeOutAndIn(controlsPanelGroup, mainMenuGroup));
    }
}