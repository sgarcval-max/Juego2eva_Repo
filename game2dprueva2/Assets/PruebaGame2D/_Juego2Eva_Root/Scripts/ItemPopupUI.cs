using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPopupUI : MonoBehaviour
{
    public static ItemPopupUI Instance;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowItem(ItemData data)
    {
        panel.SetActive(true);

        itemImage.sprite = data.icon;
        itemNameText.text = data.itemName;
        descriptionText.text = data.description;

        Time.timeScale = 0f;
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
