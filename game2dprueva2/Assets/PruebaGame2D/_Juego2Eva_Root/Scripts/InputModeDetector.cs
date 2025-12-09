using UnityEngine;
using UnityEngine.EventSystems;

public class InputModeDetector : MonoBehaviour
{
    public float axisThreshold = 0.5f;
    public string verticalAxis = "Vertical"; // puedes usar LeftStickY si prefieres
    public string horizontalAxis = "Horizontal";

    private bool usingGamepad = false;
    private Vector3 lastMousePos;

    void Start() { lastMousePos = Input.mousePosition; }

    void Update()
    {
        DetectMouse();
        DetectGamepadAxis();
    }

    void DetectMouse()
    {
        if ((Input.mousePosition - lastMousePos).sqrMagnitude > 4f)
        {
            // switch to mouse/keyboard
            if (usingGamepad)
            {
                usingGamepad = false;
                // Cancel selected gameobject so mouse can click
                EventSystem.current.SetSelectedGameObject(null);
            }
            lastMousePos = Input.mousePosition;
        }
    }

    void DetectGamepadAxis()
    {
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        // if you use LeftStickX/Y names, add checks for them too
        if (Mathf.Abs(v) > axisThreshold || Mathf.Abs(h) > axisThreshold)
        {
            if (!usingGamepad)
            {
                usingGamepad = true;
                // set first selected to some UI if needed
                // EventSystem.current.SetSelectedGameObject(defaultSelected);
            }
            // handle navigation manually (see MenuNavigation script)
        }
    }

    public bool UsingGamepad() => usingGamepad;
}
