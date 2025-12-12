using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public InputDeviceType deviceType = InputDeviceType.Keyboard;
    public KeyBindingsManager keyManager;
    public GameObject buttonPrefab; // Prefab con Button + TMP_Text
    public GameObject headerPrefab; // Prefab con TMP_Text solo para el título de sección
    public Transform contentContainer; // Content del ScrollRect (todo dentro de un mismo Scroll)

    [Header("Secciones")]
    public List<string> gameplayActions = new List<string>();
    public List<string> menuActions = new List<string>();

    private readonly string[] possibleAxisNames = new string[] {
        "LeftStickX","LeftStickY","RightStickX","RightStickY","DPadX","DPadY"
    };

    void Start()
    {
        if (keyManager == null)
            keyManager = KeyBindingsManager.Instance ?? FindObjectOfType<KeyBindingsManager>();
        if (keyManager == null)
        {
            Debug.LogError("[OptionsMenu] No KeyBindingsManager encontrado!");
            return;
        }

        // Limpiar contenido previo
        foreach (Transform t in contentContainer) Destroy(t.gameObject);

        // Crear sección Gameplay
        CreateSection("Gameplay Actions", gameplayActions);

        // Crear sección Menu
        CreateSection("Menu Actions", menuActions);
    }

    void CreateSection(string sectionTitle, List<string> actions)
    {
        // Crear header
        if (headerPrefab != null)
        {
            GameObject headerGO = Instantiate(headerPrefab, contentContainer);
            TMP_Text headerText = headerGO.GetComponent<TMP_Text>();
            if (headerText != null) headerText.text = sectionTitle;
        }

        // Crear botones de la sección
        foreach (var actionName in actions)
        {
            var ad = FindActionData(actionName);
            if (ad == null)
            {
                Debug.LogWarning($"[OptionsMenu] Acción '{actionName}' no encontrada.");
                continue;
            }

            GameObject btnGO = Instantiate(buttonPrefab, contentContainer);
            TMP_Text txt = btnGO.GetComponentInChildren<TMP_Text>();
            UpdateText(txt, actionName);

            Button btn = btnGO.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                if (deviceType == InputDeviceType.Gamepad)
                {
                    if (!string.IsNullOrEmpty(ad.defaultGamepadAxis))
                        StartCoroutine(WaitForGamepadAxis(actionName, txt));
                    else
                        StartCoroutine(WaitForKey(actionName, txt));
                }
                else
                {
                    StartCoroutine(WaitForKey(actionName, txt));
                }
            });
        }
    }

    ActionData FindActionData(string actionName)
    {
        foreach (var a in keyManager.actionList.actions)
            if (a.actionName == actionName) return a;
        return null;
    }

    void UpdateText(TMP_Text txt, string actionName)
    {
        if (txt == null) return;
        if (deviceType == InputDeviceType.Keyboard)
        {
            KeyCode kc = keyManager.GetBinding(actionName, deviceType);
            txt.text = actionName + " : " + (kc == KeyCode.None ? "None" : kc.ToString());
        }
        else
        {
            KeyCode kc = keyManager.GetBinding(actionName, deviceType);
            string axis = keyManager.GetGamepadAxisBinding(actionName);
            if (!string.IsNullOrEmpty(axis)) txt.text = actionName + " : " + axis;
            else txt.text = actionName + " : " + (kc == KeyCode.None ? "None" : kc.ToString());
        }
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
                    string s = k.ToString();
                    if (deviceType == InputDeviceType.Keyboard && s.StartsWith("Joystick")) continue;

                    keyManager.Rebind(actionName, k, deviceType);
                    txt.text = actionName + " : " + k.ToString();
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

    IEnumerator WaitForGamepadAxis(string actionName, TMP_Text txt)
    {
        txt.text = actionName + " : ... (mueve stick)";
        float threshold = 0.5f;
        bool assigned = false;
        yield return new WaitForSeconds(0.05f);

        while (!assigned)
        {
            foreach (var an in possibleAxisNames)
            {
                float val = Input.GetAxis(an);
                if (Mathf.Abs(val) > threshold)
                {
                    keyManager.RebindGamepadAxis(actionName, an);
                    txt.text = actionName + " : " + an;
                    assigned = true;
                    break;
                }
            }

            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    string s = k.ToString();
                    if (s.StartsWith("JoystickButton"))
                    {
                        keyManager.Rebind(actionName, k, InputDeviceType.Gamepad);
                        txt.text = actionName + " : " + s;
                        assigned = true;
                        break;
                    }
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