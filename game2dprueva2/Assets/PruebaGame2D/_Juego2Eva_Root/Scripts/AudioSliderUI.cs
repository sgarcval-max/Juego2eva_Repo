using UnityEngine;
using UnityEngine.UI;
using TMPro; // si usas TextMeshPro

public class AudioSliderUI : MonoBehaviour
{
    public Slider slider;
    public TMP_Text percentageText; // o Text si no usas TMP

    void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(UpdateText);
            UpdateText(slider.value);
        }
    }

    void UpdateText(float value)
    {
        if (percentageText != null)
        {
            int percent = Mathf.RoundToInt(value * 100);
            percentageText.text = percent + "%";
        }
    }
}
