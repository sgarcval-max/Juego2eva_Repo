using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public InputDeviceType deviceType = InputDeviceType.Keyboard;
    public KeyBindingsManager keyManager;
    public GameObject buttonPrefab; // prefab con Button + TMP_Text
    public Transform gameplayContainer; // container para sección Gameplay
    public Transform menuContainer;     // container para sección Menu (pause/menu controls)
    public List<string> gameplayActions = new List<string>(); // nombres de acciones que van en Gameplay
    public List<string> menuActions = new List<string>();     // nombres de acciones que van en Menu

    private readonly string[] possibleAxisNames = new string[] {
        "LeftStickX","LeftStickY","RightStickX","RightStickY","DPadX","DPadY"
    };

    void Start()
    {
        if (keyManager == null) keyManager = KeyBindingsManager.Instance ?? FindObjectOfType<KeyBindingsManager>();
        if (keyManager == null) { Debug.LogError("[OptionsMenuDevice] No KeyBindingsManager!"); return; }

        // Generar UI según deviceType y secciones configuradas
        if (deviceType == InputDeviceType.Keyboard)
        {
            PopulateSection(gameplayContainer, gameplayActions);
            PopulateSection(menuContainer, menuActions);
        }
        else // Gamepad
        {
            // En gamepad, para movement usamos MoveHorizontal (si existe en ActionList)
            PopulateSection(gameplayContainer, gameplayActions); // pero gameplayActions en inspector para gamepad debería contener MoveHorizontal en vez de MoveLeft/MoveRight
            PopulateSection(menuContainer, menuActions);
        }
    }

    // Rellena un container con botones según la lista de actionNames
    void PopulateSection(Transform container, List<string> actionNames)
    {
        if (container == null) return;

        foreach (var actionName in actionNames)
        {
            // buscar ActionData en actionList para validar
            var ad = FindActionData(actionName);
            if (ad == null) { Debug.LogWarning($"[OptionsMenuDevice] Action '{actionName}' not found in ActionListSO"); continue; }

            GameObject b = Instantiate(buttonPrefab, container);
            TMP_Text txt = b.GetComponentInChildren<TMP_Text>();
            UpdateText(txt, actionName);

            Button btn = b.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                if (deviceType == InputDeviceType.Gamepad)
                {
                    // decidir rebind boton o axis: si action has defaultGamepadAxis or user chooses, we allow axis
                    // Aquí simplificamos: si action data tiene defaultGamepadAxis no vacío, abrimos rebind axis directamente, si no rebind boton
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

    // ---------- Coroutines de rebind (igual que antes) ----------
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
                    // filtro: no joystick si teclado
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

            // fallback botones
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