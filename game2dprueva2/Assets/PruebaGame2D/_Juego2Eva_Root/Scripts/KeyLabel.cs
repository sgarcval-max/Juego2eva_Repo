using UnityEngine;
using TMPro;

public class KeyLabel : MonoBehaviour
{
    public string actionName; // "Atacar"
    public RectTransform keyE_Pos;
    public RectTransform keyQ_Pos;
    public RectTransform keyR_Pos;
    // ... añade todas las que necesites

    private TMP_Text txt;

    private void Start()
    {
        txt = GetComponent<TMP_Text>();
    }

    public void UpdateLabel(KeyCode newKey)
    {
        txt.text = actionName;

        switch (newKey)
        {
            case KeyCode.E:
                transform.position = keyE_Pos.position;
                break;

            case KeyCode.Q:
                transform.position = keyQ_Pos.position;
                break;

            case KeyCode.R:
                transform.position = keyR_Pos.position;
                break;

                // añade todos los casos que necesites
        }
    }
}