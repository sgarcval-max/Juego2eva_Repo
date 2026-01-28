using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Player")]
    public Entity playerEntity;

    [Header("Blocks UI")]
    public GameObject healthBlockPrefab;
    public Transform blocksContainer;
    public float blockSpacing = 5f;

    [Header("Colors")]
    public Color fullColor = Color.green;
    public Color lowColor = Color.red;

    private List<Image> blockImages = new List<Image>();
    private List<int> lastHealth = new List<int>();
    private List<Vector3> originalPositions = new List<Vector3>(); // posiciones originales de cada bloque

    void Start()
    {
        if (playerEntity == null)
            playerEntity = FindObjectOfType<Entity>();

        if (playerEntity == null)
        {
            Debug.LogError("PlayerHealthUI: no se encontró Entity en escena.");
            return;
        }

        if (healthBlockPrefab == null || blocksContainer == null)
        {
            Debug.LogError("PlayerHealthUI: falta prefab o contenedor.");
            return;
        }

        CreateBlocks();
        UpdateBlocksInstant();
    }

    void Update()
    {
        if (playerEntity != null)
            UpdateBlocks();
    }

    void CreateBlocks()
    {
        foreach (Transform child in blocksContainer)
            Destroy(child.gameObject);

        blockImages.Clear();
        lastHealth.Clear();
        originalPositions.Clear();

        for (int i = 0; i < playerEntity.MaxHealth; i++)
        {
            GameObject block = Instantiate(healthBlockPrefab, blocksContainer);
            RectTransform rt = block.GetComponent<RectTransform>();
            Vector3 pos = new Vector3(i * (rt.sizeDelta.x + blockSpacing), 0, 0);
            rt.anchoredPosition = pos;

            Image img = block.GetComponent<Image>();
            if (img == null)
                img = block.AddComponent<Image>();

            img.color = fullColor;
            blockImages.Add(img);
            lastHealth.Add(1);

            // Guardar posición original
            originalPositions.Add(pos);
        }
    }

    void UpdateBlocks()
    {
        int currentHealth = playerEntity.CurrentHealth;
        int maxHealth = playerEntity.MaxHealth;

        for (int i = 0; i < blockImages.Count; i++)
        {
            RectTransform rt = blockImages[i].rectTransform;

            // Si debería estar vivo
            if (i < currentHealth)
            {
                if (lastHealth[i] == 0)
                {
                    // Resetear posición y rotación antes de animar
                    rt.anchoredPosition = originalPositions[i];
                    rt.localRotation = Quaternion.identity;
                    rt.localScale = Vector3.zero;

                    StartCoroutine(AppearHeart(blockImages[i]));
                    lastHealth[i] = 1;
                }

                // Color según % vida
                float t = (float)currentHealth / maxHealth;
                blockImages[i].color = Color.Lerp(lowColor, fullColor, t);
            }
            else
            {
                // Si debería estar muerto
                if (lastHealth[i] == 1)
                {
                    StartCoroutine(DamageHeart(blockImages[i]));
                    lastHealth[i] = 0;
                }
            }
        }
    }

    void UpdateBlocksInstant()
    {
        int currentHealth = playerEntity.CurrentHealth;

        for (int i = 0; i < blockImages.Count; i++)
        {
            blockImages[i].gameObject.SetActive(i < currentHealth);
            blockImages[i].color = (i < currentHealth) ? fullColor : lowColor;
            lastHealth[i] = (i < currentHealth) ? 1 : 0;

            blockImages[i].rectTransform.localScale = Vector3.one * 0.9f;
            blockImages[i].rectTransform.localRotation = Quaternion.identity;
            blockImages[i].rectTransform.anchoredPosition = originalPositions[i];
        }
    }

    // ---------------- Animación de curación ----------------
    IEnumerator AppearHeart(Image heart)
    {
        heart.gameObject.SetActive(true);
        RectTransform rt = heart.rectTransform;

        CanvasGroup cg = heart.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = heart.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // Resetear transform
        rt.localScale = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        rt.anchoredPosition = originalPositions[blockImages.IndexOf(heart)];

        float duration = 0.4f;
        float time = 0f;
        Vector3 targetScale = Vector3.one * 0.9f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Sin(time / duration * Mathf.PI * 0.5f); // easing
            rt.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        rt.localScale = targetScale;
        rt.localRotation = Quaternion.identity;
        rt.anchoredPosition = originalPositions[blockImages.IndexOf(heart)];
    }

    // ---------------- Animación de daño ----------------
    IEnumerator DamageHeart(Image heart)
    {
        RectTransform rt = heart.rectTransform;
        CanvasGroup cg = heart.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = heart.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        float duration = 0.5f;
        float time = 0f;

        Vector3 startPos = rt.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(Random.Range(-20f, 20f), -50f, 0);
        Vector3 startScale = rt.localScale;
        Vector3 endScale = startScale * 0.5f;
        float startRot = 0f;
        float endRot = Random.Range(-45f, 45f);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            rt.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            rt.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(startRot, endRot, t));
            rt.localScale = Vector3.Lerp(startScale, endScale, t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        rt.localScale = endScale;
        rt.localRotation = Quaternion.identity;
        rt.anchoredPosition = originalPositions[blockImages.IndexOf(heart)];
        cg.alpha = 0f;
        heart.gameObject.SetActive(false);
    }
}