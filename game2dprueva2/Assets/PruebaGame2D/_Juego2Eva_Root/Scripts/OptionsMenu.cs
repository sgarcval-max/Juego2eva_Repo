using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
    public InputDeviceType deviceType = InputDeviceType.Keyboard; // set in inspector per panel
    public KeyBindingsManager keyManager;
    public GameObject buttonPrefab;
    public Transform container;

    void Start()
    {
        if (keyManager == null)
        {
            if (KeyBindingsManager.Instance != null) keyManager = KeyBindingsManager.Instance;
            else keyManager = FindObjectOfType<KeyBindingsManager>();
        }

        if (keyManager == null) { Debug.LogError("[OptionsMenuDevice] No KeyBindingsManager!"); return; }

        foreach (var action in keyManager.actionList.actions)
        {
            GameObject btnObj = Instantiate(buttonPrefab, container);
            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            string actionName = action.actionName;

            UpdateButtonText(txt, actionName);

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                StartCoroutine(WaitForKey(actionName, txt));
            });
        }
    }

    void UpdateButtonText(TMP_Text txt, string action)
    {
        KeyCode code = keyManager.GetBinding(action, deviceType);
        txt.text = action + " : " + KeyCodeToNiceString(code);
    }

    IEnumerator WaitForKey(string action, TMP_Text txt)
    {
        txt.text = action + " : ...";
        yield return null; yield return null; // evitar click registrado

        bool assigned = false;
        while (!assigned)
        {
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    // solo aceptar controles relevantes: si deviceType == Keyboard, ignorar Joystick keys; si Gamepad, aceptar Joystick only (opcional)
                    if (deviceType == InputDeviceType.Keyboard)
                    {
                        // ignorar joystick button asignaciones si no quieres permitirlos en el panel Keyboard
                        string s = k.ToString();
                        if (s.StartsWith("Joystick")) continue;
                    }
                    else // Gamepad
                    {
                        // solo aceptar joystick buttons (o también teclas si quieres)
                        string s = k.ToString();
                        if (!s.StartsWith("Joystick")) continue;
                    }

                    keyManager.Rebind(action, k, deviceType);
                    UpdateButtonText(txt, action);
                    assigned = true;
                    break;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // cancelar
                UpdateButtonText(txt, action);
                yield break;
            }
            yield return null;
        }
    }

    private string KeyCodeToNiceString(KeyCode kc)
    {
        if (kc == KeyCode.None) return "None";
        string s = kc.ToString();
        if (s.StartsWith("JoystickButton")) return "Gamepad B" + s.Replace("JoystickButton", "");
        if (s.StartsWith("Joystick")) return s;
        // mouse
        if (kc == KeyCode.Mouse0) return "Mouse L";
        return s.ToUpper();
    }
}