using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Player")]
    public Entity playerEntity;           // referencia al Player

    [Header("Blocks UI")]
    public GameObject healthBlockPrefab;  // prefab del bloque de vida
    public Transform blocksContainer;     // contenedor de los bloques
    public float blockSpacing = 5f;       // separación entre bloques

    [Header("Colors")]
    public Color fullColor = Color.green; // color inicial de todos los bloques
    public Color lowColor = Color.red;    // color cuando queda poca vida

    private List<Image> blockImages = new List<Image>();

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
        UpdateBlocks();
    }

    void Update()
    {
        if (playerEntity != null)
            UpdateBlocks();
    }

    void CreateBlocks()
    {
        // limpiar bloques antiguos
        foreach (Transform child in blocksContainer)
            Destroy(child.gameObject);

        blockImages.Clear();

        // crear bloques según MaxHealth
        for (int i = 0; i < playerEntity.MaxHealth; i++)
        {
            GameObject block = Instantiate(healthBlockPrefab, blocksContainer);
            RectTransform rt = block.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * (rt.sizeDelta.x + blockSpacing), 0);

            Image img = block.GetComponent<Image>();
            if (img == null)
                img = block.AddComponent<Image>();

            img.color = fullColor; // todos empiezan verdes
            blockImages.Add(img);
        }
    }

    void UpdateBlocks()
    {
        int currentHealth = playerEntity.CurrentHealth;
        int maxHealth = playerEntity.MaxHealth;

        for (int i = 0; i < blockImages.Count; i++)
        {
            // activar/desactivar según la vida actual
            blockImages[i].gameObject.SetActive(i < currentHealth);

            if (i < currentHealth)
            {
                // cambiar color de verde a rojo según vida restante
                float t = (float)currentHealth / maxHealth; // porcentaje de vida
                blockImages[i].color = Color.Lerp(lowColor, fullColor, t);
            }
        }
    }
}