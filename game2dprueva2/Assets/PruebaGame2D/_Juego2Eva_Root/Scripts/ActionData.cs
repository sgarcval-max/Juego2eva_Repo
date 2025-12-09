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
    public KeyCode defaultKey;
    public string defaultGamepadButton; // ej "JoystickButton0"
    public string defaultGamepadAxis;   // ej "LeftStickX" o "" si no aplica
}