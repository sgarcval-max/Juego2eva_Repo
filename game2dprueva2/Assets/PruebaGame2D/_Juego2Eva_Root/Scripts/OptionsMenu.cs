using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
    public InputDeviceType deviceType = InputDeviceType.Keyboard;
    public KeyBindingsManager keyManager;
    public GameObject buttonPrefab; // prefab must contain Button + TMP_Text child
    public Transform container;

    void Start()
    {
        if (keyManager == null) keyManager = KeyBindingsManager.Instance ?? FindObjectOfType<KeyBindingsManager>();
        if (keyManager == null) { Debug.LogError("[OptionsMenuDevice] No KeyBindingsManager!"); return; }

        foreach (var action in keyManager.actionList.actions)
        {
            GameObject b = Instantiate(buttonPrefab, container);
            TMP_Text txt = b.GetComponentInChildren<TMP_Text>();
            string actionName = action.actionName;
            UpdateText(txt, actionName);

            Button btn = b.GetComponent<Button>();
            btn.onClick.AddListener(() => { StartCoroutine(WaitForKey(actionName, txt)); });
        }
    }

    void UpdateText(TMP_Text txt, string actionName)
    {
        KeyCode kc = keyManager.GetBinding(actionName, deviceType);
        txt.text = actionName + " : " + (kc == KeyCode.None ? "None" : kc.ToString());
    }

    IEnumerator WaitForKey(string actionName, TMP_Text txt)
    {
        txt.text = actionName + " : ...";
        yield return null; yield return null;

        bool assigned = false;
        while (!assigned)
        {
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    // filter by deviceType
                    string s = k.ToString();
                    if (deviceType == InputDeviceType.Keyboard && s.StartsWith("Joystick")) continue;
                    if (deviceType == InputDeviceType.Gamepad && !s.StartsWith("Joystick")) continue;

                    keyManager.Rebind(actionName, k, deviceType);
                    UpdateText(txt, actionName);
                    assigned = true;
                    break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UpdateText(txt, actionName);
                yield break;
            }
            yield return null;
        }
    }
}