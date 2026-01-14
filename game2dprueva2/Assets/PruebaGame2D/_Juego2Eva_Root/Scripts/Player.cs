using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;

    private float xInput;
    private bool canJump = true;

    // Nueva propiedad pública para consultar si está muerto
    public bool IsDead => currentHealth <= 0;

    private void OnEnable()
    {
        if (GameInput.Instance != null)
            GameInput.Instance.inputActions.Player.Enable();
    }

    protected override void Update()
    {
        base.Update();
        HandleInput();
    }

    private void HandleInput()
    {
        xInput = GetHorizontalInput();

        if (GameInput.Instance.inputActions.Player.Jump.triggered)
            TryToJump();

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
        Vector2 gamepadInput = GameInput.Instance.inputActions.Player.Move.ReadValue<Vector2>();
        if (Mathf.Abs(gamepadInput.x) > 0.12f)
            return gamepadInput.x;

        float h = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;
        return h;
    }

    public new void TakeDamage(int amount = 1)
    {
        base.TakeDamage(amount);

        if (PlayerHealthManager.Instance != null)
            PlayerHealthManager.Instance.UpdateHealth(currentHealth);

        if (currentHealth <= 0 && PlayerHealthManager.Instance != null)
            PlayerHealthManager.Instance.ResetHealth();
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, MaxHealth);
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

}