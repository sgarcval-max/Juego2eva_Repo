using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MenuNavigation : MonoBehaviour
{
    [Header("Navigation Settings")]
    public float stickRepeatDelay = 0.25f;
    public Selectable defaultSelected;

    [Header("Audio")]
    public AudioClip sfxMove;
    public AudioClip sfxConfirm;

    private float nextMoveTime = 0f;
    private float nextMoveSFXTime = 0f; // Limitar SFX move
    private EventSystem es;
    private AudioSource audioSource;

    private string[] verticalAxes = new string[] { "Vertical", "LeftStickY", "DPadY" };
    private string[] horizontalAxes = new string[] { "Horizontal", "LeftStickX", "DPadX" };

    [Header("SFX Cooldown")]
    public float moveSFXCooldown = 0.1f; // mínimo tiempo entre reproducir el SFX de move

    void Awake()
    {
        es = EventSystem.current;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            return;

        if (Input.mousePresent && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
            return;

        Vector2 move = Vector2.zero;

        foreach (var a in horizontalAxes)
        {
            float hv = Input.GetAxisRaw(a);
            if (Mathf.Abs(hv) > 0.5f)
            {
                move.x = hv;
                break;
            }
        }
        foreach (var a in verticalAxes)
        {
            float vv = Input.GetAxisRaw(a);
            if (Mathf.Abs(vv) > 0.5f)
            {
                move.y = vv;
                break;
            }
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

        // -------------------------
        //   CONFIRMAR
        // -------------------------
        if ((Input.GetButtonDown("Submit") && !Input.GetKeyDown(KeyCode.Space))
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            GameObject sel = es.currentSelectedGameObject;
            if (sel != null)
            {
                var btn = sel.GetComponent<Button>();
                if (btn != null) btn.onClick.Invoke();
                PlayConfirm();
            }
        }
    }

    void MoveSelection(Vector2 dir)
    {
        if (es == null) return;

        GameObject current = es.currentSelectedGameObject;
        if (current == null)
        {
            if (defaultSelected != null)
                es.SetSelectedGameObject(defaultSelected.gameObject);
            return;
        }

        Selectable sel = current.GetComponent<Selectable>();
        if (sel == null) return;

        Selectable next = null;

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            next = (dir.y > 0) ? sel.FindSelectableOnUp() : sel.FindSelectableOnDown();
        else
            next = (dir.x > 0) ? sel.FindSelectableOnRight() : sel.FindSelectableOnLeft();

        if (next != null)
        {
            es.SetSelectedGameObject(next.gameObject);
            TryPlayMove();
        }
    }

    // -------------------- MÉTODOS DE AUDIO --------------------

    public void PlayConfirm()
    {
        if (sfxConfirm != null && audioSource != null)
            audioSource.PlayOneShot(sfxConfirm);
    }

    public void TryPlayMove()
    {
        if (sfxMove == null || audioSource == null) return;

        // Limitar SFX de movimiento con cooldown
        if (Time.unscaledTime < nextMoveSFXTime) return;

        audioSource.PlayOneShot(sfxMove);
        nextMoveSFXTime = Time.unscaledTime + moveSFXCooldown;
    }

    /// <summary>
    /// Método seguro para EventTrigger → PointerEnter
    /// </summary>
    public void TryPlayMoveEvent()
    {
        TryPlayMove();
    }
}