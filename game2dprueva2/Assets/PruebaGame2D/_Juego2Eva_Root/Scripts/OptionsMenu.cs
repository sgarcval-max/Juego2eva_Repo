using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public KeyBindingsManager keyManager;
    public GameObject buttonPrefab;
    public Transform container;

    private void Start()
    {
        // Si no arrastraste el KeyManager en el Inspector, lo buscamos por singleton o por escena
        if (keyManager == null)
        {
            if (KeyBindingsManager.Instance != null)
                keyManager = KeyBindingsManager.Instance;
            else
                keyManager = FindObjectOfType<KeyBindingsManager>(); // último recurso
        }

        if (keyManager == null)
        {
            Debug.LogError("[OptionsMenu] No KeyBindingsManager found! Asegúrate de tener un KeyBindingsManager persistente.");
            return;
        }

        // resto del Start original: generar botones...
        foreach (var action in keyManager.actionList.actions)
        {
            GameObject btnObj = Instantiate(buttonPrefab, container);
            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();

            UpdateButtonText(txt, action.actionName);

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                StartCoroutine(WaitForKey(action.actionName, txt));
            });
        }
    }

    private void UpdateButtonText(TMP_Text txt, string action)
    {
        txt.text = action + " : " + keyManager.GetBinding(action).ToString();
    }

    private System.Collections.IEnumerator WaitForKey(string action, TMP_Text txt)
    {
        txt.text = "Press any key...";

        while (!Input.anyKeyDown)
            yield return null;

        foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(k))
            {
                keyManager.Rebind(action, k);
                UpdateButtonText(txt, action);
                break;
            }
        }
    }
}