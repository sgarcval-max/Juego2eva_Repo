using UnityEngine;

[System.Serializable]
public enum InputType
{
    Keyboard,
    Mouse,
    GamepadButton
}

[System.Serializable]
public class ActionData
{
    public string actionName;

    // Tecla del teclado
    public KeyCode defaultKey;

    // Botón del gamepad
    public string defaultGamepadButton;

    // Tipo de input
    public InputType inputType;
}