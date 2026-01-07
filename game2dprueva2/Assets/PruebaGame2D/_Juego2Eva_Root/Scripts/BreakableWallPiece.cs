using UnityEngine;
using System.Collections;

public class BreakableWallPiece : MonoBehaviour
{
    [Header("Lifetime & Fade")]
    public float lifetime = 3f;    // tiempo hasta empezar a desaparecer
    public float fadeDuration = 1f; // duración del fundido

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogWarning("BreakableWallPieceFade necesita un SpriteRenderer");
    }

    private void OnEnable()
    {
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        // Esperar el tiempo de vida
        yield return new WaitForSeconds(lifetime);

        float t = 0f;
        Color originalColor = sr.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}
