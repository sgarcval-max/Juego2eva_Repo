using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MenuNavigation : MonoBehaviour
{
    public float stickRepeatDelay = 0.25f;
    public Selectable defaultSelected;
    public AudioClip sfxMove;
    public AudioClip sfxConfirm;

    private float nextMoveTime = 0f;
    private EventSystem es;
    private AudioSource audioSource;

    // Ejes alternativos que podrían usarse para navegar (añade los que tengas)
    private string[] verticalAxes = new string[] { "Vertical", "LeftStickY", "DPadY" };
    private string[] horizontalAxes = new string[] { "Horizontal", "LeftStickX", "DPadX" };

    void Awake()
    {
        es = EventSystem.current;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // si hay movimiento de ratón, dejamos que el mouse controle
        if (Input.mousePresent && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
            return;

        Vector2 move = Vector2.zero;

        // leer ejes alternativos
        foreach (var a in horizontalAxes)
        {
            float hv = Input.GetAxisRaw(a);
            if (Mathf.Abs(hv) > 0.5f) { move.x = hv; break; }
        }
        foreach (var a in verticalAxes)
        {
            float vv = Input.GetAxisRaw(a);
            if (Mathf.Abs(vv) > 0.5f) { move.y = vv; break; }
        }

        if (move.sqrMagnitude > 0.01f)
        {
            if (Time.unscaledTime > nextMoveTime)
            {
                MoveSelection(move);
                nextMoveTime = Time.unscaledTime + stickRepeatDelay;
            }
        }
        else
        {
            nextMoveTime = 0f;
        }

        // Confirm (acepta JoystickButton0 y Submit)
        if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            GameObject sel = es.currentSelectedGameObject;
            if (sel != null)
            {
                var btn = sel.GetComponent<Button>();
                if (btn != null) btn.onClick.Invoke();
                if (sfxConfirm != null) audioSource.PlayOneShot(sfxConfirm);
            }
        }
    }

    void MoveSelection(Vector2 dir)
    {
        if (es == null) return;
        GameObject current = es.currentSelectedGameObject;
        if (current == null)
        {
            if (defaultSelected != null) es.SetSelectedGameObject(defaultSelected.gameObject);
            return;
        }

        Selectable sel = current.GetComponent<Selectable>();
        if (sel == null) return;

        Selectable next = null;
        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0) next = sel.FindSelectableOnUp();
            else next = sel.FindSelectableOnDown();
        }
        else
        {
            if (dir.x > 0) next = sel.FindSelectableOnRight();
            else next = sel.FindSelectableOnLeft();
        }

        if (next != null)
        {
            es.SetSelectedGameObject(next.gameObject);
            if (sfxMove != null) audioSource.PlayOneShot(sfxMove);
        }
    }
}