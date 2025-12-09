using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public InputDeviceType deviceType = InputDeviceType.Keyboard;
    public KeyBindingsManager keyManager;
    public GameObject buttonPrefab; // prefab con Button + TMP_Text
    public Transform container;

    // Ejes que el juego tenga en InputManager. Ajústalos a tu Input Manager.
    private readonly string[] possibleAxisNames = new string[] {
        "LeftStickX","LeftStickY","RightStickX","RightStickY","DPadX","DPadY"
    };

    private void Start()
    {
        if (keyManager == null) keyManager = KeyBindingsManager.Instance ?? FindObjectOfType<KeyBindingsManager>();
        if (keyManager == null) { Debug.LogError("[OptionsMenuDevice] No KeyBindingsManager!"); return; }

        // Generar botones por cada acción
        foreach (var action in keyManager.actionList.actions)
        {
            GameObject b = Instantiate(buttonPrefab, container);
            TMP_Text txt = b.GetComponentInChildren<TMP_Text>();
            string actionName = action.actionName;
            UpdateText(txt, actionName);

            Button btn = b.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // Si estamos en Gamepad, abrimos opción: reasignar botón o axis
                if (deviceType == InputDeviceType.Gamepad)
                {
                    // Puedes mostrar UI para elegir "Botón" o "Stick". Aquí simplificamos: si el usuario mantiene SHIFT reasignamos axis, sino botón.
                    // Para control más limpio deberías mostrar un diálogo.
                    StartCoroutine(WaitForGamepadChoice(actionName, txt));
                }
                else
                {
                    StartCoroutine(WaitForKey(actionName, txt));
                }
            });
        }
    }

    void UpdateText(TMP_Text txt, string actionName)
    {
        if (deviceType == InputDeviceType.Keyboard)
        {
            KeyCode kc = keyManager.GetBinding(actionName, deviceType);
            txt.text = actionName + " : " + (kc == KeyCode.None ? "None" : kc.ToString());
        }
        else // Gamepad
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
        // evitar que el click del ratón se capture
        yield return null;
        yield return null;

        bool assigned = false;
        while (!assigned)
        {
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    // filtro: para teclado solo aceptar no-Joystick
                    string s = k.ToString();
                    if (deviceType == InputDeviceType.Keyboard && s.StartsWith("Joystick")) continue;
                    // asignar
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

    // Decide si reasignar boton o axis; aquí mostramos un modo simple:
    IEnumerator WaitForGamepadChoice(string actionName, TMP_Text txt)
    {
        // Mostramos texto indicando modo
        txt.text = actionName + " : Presiona 'A' para boton o mueve stick para axis (Esc cancelar)";
        // Espera un frame
        yield return null;

        float timeout = 3f; // esperar unos segundos para elegir
        float t = 0f;
        while (t < timeout)
        {
            t += Time.unscaledDeltaTime;

            // Si detecta movimiento de axis fuerte, inicia rebind axis inmediatamente
            foreach (var an in possibleAxisNames)
            {
                float val = Input.GetAxis(an);
                if (Mathf.Abs(val) > 0.5f)
                {
                    // asignar axis
                    yield return StartCoroutine(WaitForGamepadAxis(actionName, txt));
                    yield break;
                }
            }

            // Si detecta botón joystick presionado, reasignar boton
            foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    string s = k.ToString();
                    if (s.StartsWith("JoystickButton"))
                    {
                        keyManager.Rebind(actionName, k, InputDeviceType.Gamepad);
                        txt.text = actionName + " : " + s;
                        yield break;
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

        // si no eligió, volvemos a mostrar texto actual
        UpdateText(txt, actionName);
    }

    // Detectar axis: mover stick o pad
    public IEnumerator WaitForGamepadAxis(string actionName, TMP_Text txt)
    {
        txt.text = actionName + " : ... (mueve stick)";

        float threshold = 0.5f;
        bool assigned = false;
        // esperar un pelin para evitar ruido
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

            // también aceptamos botón como fallback
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