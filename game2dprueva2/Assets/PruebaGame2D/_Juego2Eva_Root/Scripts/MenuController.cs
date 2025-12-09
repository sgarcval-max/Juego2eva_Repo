using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Header("UI refs")]
    public CanvasGroup fadePanel; // Fade panel canvasgroup (negro)
    public Animator titleAnimator; // Animator del TitlePanel
    public RectTransform titleRect; // RectTransform del TitlePanel
    public Animator[] buttonAnimators; // Animators de cada botón (orden: Play, Options, Exit)
    public GameObject firstSelected; // botón seleccionado inicialmente (Play)

    [Header("Timings")]
    public float initialFadeDuration = 1f;
    public float titleAppearDelay = 0.2f;
    public float betweenButtonsDelay = 0.12f;

    [Header("Press Space Message (Blink)")]
    public GameObject pressSpaceText;
    public float blinkSpeed = 0.8f;
    private Coroutine blinkCoroutine = null;

    [Header("Main Menu")]
    public CanvasGroup mainMenuGroup; // Solo el panel principal de la escena

    private bool inputEnabled = false;

    void Start()
    {
        if (fadePanel != null) { fadePanel.alpha = 1f; fadePanel.blocksRaycasts = true; }
        if (buttonAnimators != null) { foreach (var b in buttonAnimators) if (b != null) b.gameObject.SetActive(true); }
        if (pressSpaceText != null) pressSpaceText.SetActive(false);

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

        if (titleAnimator != null) titleAnimator.SetTrigger("Appear");
        yield return new WaitForSecondsRealtime(0.9f);

        inputEnabled = true;
        if (pressSpaceText != null) { pressSpaceText.SetActive(true); blinkCoroutine = StartCoroutine(BlinkTextCoroutine()); }
        if (firstSelected != null && EventSystem.current != null) EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private IEnumerator BlinkTextCoroutine()
    {
        while (true)
        {
            if (pressSpaceText != null) pressSpaceText.SetActive(!pressSpaceText.activeSelf);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    void Update()
    {
        if (!inputEnabled) return;
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space))
        {
            if (blinkCoroutine != null) { StopCoroutine(blinkCoroutine); blinkCoroutine = null; }
            if (pressSpaceText != null) pressSpaceText.SetActive(false);
            inputEnabled = false;
            StartCoroutine(PlayTitleToMenu());
        }
    }

    IEnumerator PlayTitleToMenu()
    {
        if (titleAnimator != null) titleAnimator.SetTrigger("Disappear");
        yield return new WaitForSecondsRealtime(0.6f);
        if (titleAnimator != null) titleAnimator.SetTrigger("ToSmall");
        yield return new WaitForSecondsRealtime(0.6f);
        if (titleAnimator != null) titleAnimator.SetTrigger("Appear");
        yield return new WaitForSecondsRealtime(0.5f);

        if (buttonAnimators != null)
        {
            for (int i = 0; i < buttonAnimators.Length; i++)
            {
                var anim = buttonAnimators[i];
                if (anim != null) anim.SetTrigger("Show");
                yield return new WaitForSecondsRealtime(betweenButtonsDelay);
            }
        }

        if (firstSelected != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void OnPlayPressed(string IntroCutscene1)
    {
        // Llama a LoadScene con fade
        StartCoroutine(LoadSceneWithFade(IntroCutscene1));
    }

    private IEnumerator LoadSceneWithFade(string IntroCutscene1)
    {
        if (fadePanel != null) fadePanel.blocksRaycasts = true;
        float t = 0f;
        while (t < initialFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            if (fadePanel != null) fadePanel.alpha = t / initialFadeDuration;
            yield return null;
        }
        if (!string.IsNullOrEmpty(IntroCutscene1))
            UnityEngine.SceneManagement.SceneManager.LoadScene(IntroCutscene1);
    }

    public void OnExitPressed() { Application.Quit(); }

    // -------------------- OPTIONS --------------------
    public void OnOptionsPressed()
    {
        if (MenuControllerRuntime.Instance != null)
        {
            MenuControllerRuntime.Instance.mainMenuGroup = mainMenuGroup; // Pasamos referencia del MainMenuGroup
            MenuControllerRuntime.Instance.ShowControlsPanel();
        }
    }
}

