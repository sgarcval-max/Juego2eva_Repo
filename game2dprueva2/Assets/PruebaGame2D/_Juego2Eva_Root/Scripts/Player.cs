using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;

    private float xInput;
    private bool canJump = true;

    private void OnEnable()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.inputActions.Player.Enable();
        }
    }

    protected override void Update()
    {
        base.Update();
        HandleInput();
    }

    private void HandleInput()
    {
        xInput = GetHorizontalInput();

        // salto
        if (GameInput.Instance.inputActions.Player.Jump.triggered)
            TryToJump();

        // ataque
        if (GameInput.Instance.inputActions.Player.Attack.triggered)
            HandleAttack();
    }

    protected override void HandleMovement()
    {
        if (canMove)
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void TryToJump()
    {
        if (isGrounded && canJump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public override void EnableMovement(bool enable)
    {
        base.EnableMovement(enable);
        canJump = enable;
    }

    protected override void Die()
    {
        base.Die();
        UI.instance.EnableGameOverUI();
    }

    public void PlayCutsceneMovement(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        if (anim != null)
        {
            anim.SetFloat("xVelocity", velocity.x);
            anim.SetFloat("yVelocity", velocity.y);
            anim.SetBool("isGrounded", true);
        }
    }

    private float GetHorizontalInput()
    {
        // 1) Gamepad
        Vector2 gamepadInput = GameInput.Instance.inputActions.Player.Move.ReadValue<Vector2>();
        if (Mathf.Abs(gamepadInput.x) > 0.12f)
            return gamepadInput.x;

        // 2) Teclado
        float h = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;

        return h;
    }

    // -------------------- HABILIDADES --------------------
    [Header("Abilities")]
    [SerializeField] private bool canDoubleJump = false;
    [SerializeField] private bool canDash = false;
    [SerializeField] private bool canFireball = false;

    public void UnlockAbility(AbilityType ability)
    {
        Debug.Log("Habilidad desbloqueada: " + ability);
        // Aquí luego activas doble salto, dash, etc.
    }

    public override void TakeDamage(int amount = 1)
    {
        base.TakeDamage(amount);

        // avisamos al manager si seguimos vivos
        if (currentHealth > 0 && PlayerHealthManager.Instance != null)
            PlayerHealthManager.Instance.UpdateHealth(currentHealth);

        // morir resetea vida
        if (currentHealth <= 0 && PlayerHealthManager.Instance != null)
        {
            PlayerHealthManager.Instance.ResetHealth(); // reaparece con vida máxima
        }
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, MaxHealth);
    }

    protected virtual void Start()
    {
        if (PlayerHealthManager.Instance != null)
        {
            SetHealth(PlayerHealthManager.Instance.GetSavedHealth());
        }
    }
}