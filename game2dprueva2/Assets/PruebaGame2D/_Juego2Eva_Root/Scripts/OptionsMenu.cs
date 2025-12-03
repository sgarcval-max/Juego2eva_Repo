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