using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar Instance;

    public Slider slider;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealth(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
