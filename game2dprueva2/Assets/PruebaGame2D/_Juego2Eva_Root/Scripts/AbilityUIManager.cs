using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : MonoBehaviour
{
    public static AbilityUIManager Instance;

    [Header("Iconos de habilidades")]
    public List<Image> abilityIcons; // imágenes de las habilidades
    public float popScale = 1.3f;    // cuánto escala el pop
    public float popDuration = 0.3f; // duración del pop

    // Para guardar qué habilidades se han desbloqueado
    private bool[] unlockedAbilities;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializar estado
            unlockedAbilities = new bool[abilityIcons.Count];
            RefreshUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Reasignar iconos al cargar nueva escena (si quieres reconstruir UI en otra escena)
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Si tus iconos están en este Canvas persistente, no hace falta reasignarlos.
        // Solo refrescamos la UI por si hay cambios
        RefreshUI();
    }

    // Llamar para desbloquear una habilidad y animar su icono
    public void UnlockAbility(int abilityIndex)
    {
        if (abilityIndex < 0 || abilityIndex >= abilityIcons.Count) return;
        if (unlockedAbilities[abilityIndex]) return; // ya desbloqueada

        unlockedAbilities[abilityIndex] = true;
        RefreshUI();
        StartCoroutine(PopIcon(abilityIcons[abilityIndex]));
    }

    // Animación de pop
    private IEnumerator PopIcon(Image icon)
    {
        if (icon == null) yield break;

        Vector3 originalScale = icon.rectTransform.localScale;
        Vector3 targetScale = originalScale * popScale;

        // Escalar hacia el pop
        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            icon.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t / popDuration);
            yield return null;
        }

        // Volver a la escala original
        t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            icon.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t / popDuration);
            yield return null;
        }

        icon.rectTransform.localScale = originalScale;
    }

    // Actualizar la UI según las habilidades desbloqueadas
    private void RefreshUI()
    {
        for (int i = 0; i < abilityIcons.Count; i++)
        {
            if (abilityIcons[i] != null)
            {
                abilityIcons[i].color = unlockedAbilities[i] ? Color.white : new Color(1f, 1f, 1f, 0.3f);
            }
        }
    }

    // Función para cargar el estado guardado externamente si quieres
    public void SetUnlockedAbilities(bool[] abilities)
    {
        if (abilities.Length != unlockedAbilities.Length) return;
        unlockedAbilities = abilities;
        RefreshUI();
    }

    public bool[] GetUnlockedAbilities()
    {
        return unlockedAbilities;
    }
}
