using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance;

    public Slider slider;
    public float refillSpeed = 1.5f;

    private void Awake()
    {
        Instance = this;
        slider.gameObject.SetActive(false);
    }

    public void Show()
    {
        slider.gameObject.SetActive(true);
    }

    public void Hide()
    {
        slider.gameObject.SetActive(false);
    }

    public void UpdateHealth(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }

    // 🔥 ANIMACIÓN DE RECUPERAR VIDA ENTRE FASES
    public void AnimateRefill(float maxHealth)
    {
        StopAllCoroutines();
        StartCoroutine(RefillRoutine(maxHealth));
    }

    IEnumerator RefillRoutine(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = 0f;

        while (slider.value < maxHealth)
        {
            slider.value += refillSpeed * Time.deltaTime * maxHealth;
            yield return null;
        }

        slider.value = maxHealth;
    }
}