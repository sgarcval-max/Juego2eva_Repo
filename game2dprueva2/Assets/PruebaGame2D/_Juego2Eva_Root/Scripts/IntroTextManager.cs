using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSceneManager : MonoBehaviour
{
    [Header("Texto")]
    public Text introText;
    [TextArea]
    public string fullText;
    public float letterDelay = 0.05f;

    [Header("Audio escritura")]
    public AudioSource typeAudio;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    [Header("Botón")]
    public GameObject continueButton;
    public string nextSceneName;

    private bool skipText = false;
    private bool typingFinished = false;

    private void Start()
    {
        continueButton.SetActive(false);
        StartCoroutine(SceneRoutine());
    }

    private void Update()
    {
        // Pulsar ESPACIO = saltar texto
        if (Input.GetKeyDown(KeyCode.Space) && !typingFinished)
        {
            skipText = true;
        }
    }

    IEnumerator SceneRoutine()
    {
        fadeCanvas.alpha = 1f;
        yield return StartCoroutine(Fade(1f, 0f));

        yield return StartCoroutine(TypeText());

        typingFinished = true;
        continueButton.SetActive(true);
    }

    IEnumerator TypeText()
    {
        introText.text = "";

        foreach (char c in fullText)
        {
            // Si se pulsa espacio → mostrar texto completo
            if (skipText)
            {
                introText.text = fullText;
                yield break;
            }

            introText.text += c;

            if (typeAudio != null && typeAudio.clip != null && c != ' ')
                typeAudio.PlayOneShot(typeAudio.clip);

            yield return new WaitForSeconds(letterDelay);
        }
    }

    public void OnContinuePressed()
    {
        StartCoroutine(ContinueRoutine());
    }

    IEnumerator ContinueRoutine()
    {
        continueButton.SetActive(false);
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0f;
        fadeCanvas.alpha = start;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = end;
    }
}

