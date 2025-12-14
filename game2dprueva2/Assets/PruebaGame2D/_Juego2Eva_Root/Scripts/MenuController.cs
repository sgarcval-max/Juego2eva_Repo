using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [Header("UI refs")]
    public CanvasGroup fadePanel;
    public Animator titleAnimator;
    public RectTransform titleRect;
    public Animator[] buttonAnimators;
    public GameObject firstSelected;

    [Header("Timings")]
    public float initialFadeDuration = 1f;
    public float titleAppearDelay = 0.2f;
    public float betweenButtonsDelay = 0.12f;

    [Header("Press Space Message (Blink)")]
    public GameObject pressSpaceText;
    public float blinkSpeed = 0.8f;
    public float holdTime = 0.15f; // Tiempo que se mantiene al llegar a 0 o 1
    private Coroutine blinkCoroutine = null;

    [Header("Main Menu")]
    public CanvasGroup mainMenuGroup; // Solo el panel principal de la escena

    private bool inputEnabled = false;

    void Start()
    {
        if (fadePanel != null) { fadePanel.alpha = 1f; fadePanel.blocksRaycasts = true; }
        if (buttonAnimators != null) { foreach (var b in buttonAnimators) if (b != null) b.gameObject.SetActive(true); }
        if (pressSpaceText != null) pressSpaceText.SetActive(false);

        // Pasar MainMenuGroup local al Runtime
        if (MenuControllerRuntime.Instance != null)
        {
            MenuControllerRuntime.Instance.mainMenuGroup = mainMenuGroup;
        }

        StartCoroutine(StartupSequence());
    }

    IEnumerator StartupSequence()
    {
        // ------------------ FADE IN FLUIDO ------------------
        if (fadePanel != null)
        {
            while (fadePanel.alpha > 0f)
            {
                fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, 0f, Time.deltaTime / initialFadeDuration);
                yield return null;
            }
            fadePanel.blocksRaycasts = false;
        }

        yield return new WaitForSecondsRealtime(titleAppearDelay);

        if (titleAnimator != null) titleAnimator.SetTrigger("Appear");
        yield return new WaitForSecondsRealtime(0.9f);

        // ------------------ ACTIVAR INPUT ------------------
        inputEnabled = true;

        // ------------------ MOSTRAR TEXTO CON FUNDIDO ------------------
        if (pressSpaceText != null)
        {
            pressSpaceText.SetActive(true);
            CanvasGroup textCG = pressSpaceText.GetComponent<CanvasGroup>();
            if (textCG == null)
            {
                textCG = pressSpaceText.AddComponent<CanvasGroup>();
                textCG.alpha = 0f;
            }
            blinkCoroutine = StartCoroutine(FadeTextLoop(textCG));
        }

        if (firstSelected != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private IEnumerator FadeTextLoop(CanvasGroup cg)
    {
        while (true)
        {
            // ------------------ FADE IN ------------------
            while (cg.alpha < 1f)
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, 1f, Time.deltaTime / blinkSpeed);
                yield return null;
            }
            yield return new WaitForSecondsRealtime(holdTime); // Espera al llegar a 1

            // ------------------ FADE OUT ------------------
            while (cg.alpha > 0f)
            {
                cg.alpha = Mathf.MoveTowards(cg.alpha, 0f, Time.deltaTime / blinkSpeed);
                yield return null;
            }
            yield return new WaitForSecondsRealtime(holdTime); // Espera al llegar a 0
        }
    }

    void Update()
    {
        if (!inputEnabled) return;

        // SOLO barra espaciadora
        if (Input.GetKeyDown(KeyCode.Space))
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
        StartCoroutine(DoSceneLoad(IntroCutscene1));
    }

    IEnumerator DoSceneLoad(string IntroCutscene1)
    {
        if (fadePanel != null) fadePanel.blocksRaycasts = true;

        float dur = 0.9f;
        float t = 0;

        // Fade out de música sincronizado
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeMusic(0f, dur);
        }

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            if (fadePanel != null) fadePanel.alpha = t / dur; // Fade in del panel
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
            MenuControllerRuntime.Instance.mainMenuGroup = mainMenuGroup;
            MenuControllerRuntime.Instance.ShowControlsPanel();
        }
    }
}