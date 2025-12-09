using UnityEngine;

public class Player : Entity
{
    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;
    private float xInput;
    private bool canJump = true;

    protected override void Update()
    {
        base.Update();
        HandleInput();
    }

    private void HandleInput()
    {
        xInput = 0;
        // movimiento horizontal (teclado sigue usando axis o keys)
        // ejemplo usando bindings MoveLeft/MoveRight:
        if (Input.GetKey(KeyBindingsManager.Instance.GetBinding("MoveRight", InputDeviceType.Keyboard))) xInput += 1;
        if (Input.GetKey(KeyBindingsManager.Instance.GetBinding("MoveLeft", InputDeviceType.Keyboard))) xInput -= 1;

        // salto: comprobamos primero gamepad effective if connected
        KeyCode jumpKey = KeyBindingsManager.Instance.GetEffectiveBinding("Jump");
        if (Input.GetKeyDown(jumpKey))
            TryToJump();

        // ataque
        KeyCode attackKey = KeyBindingsManager.Instance.GetEffectiveBinding("Attack");
        if (Input.GetKeyDown(attackKey))
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
        // Mover al player
        rb.linearVelocity = velocity;

        // Actualizar Animator para que haga animación de caminar
        if (anim != null)
        {
            anim.SetFloat("xVelocity", velocity.x);
            anim.SetFloat("yVelocity", velocity.y);
            anim.SetBool("isGrounded", true);
        }
    }

    private void HandleDash()
    {
        // Aquí puedes añadir la lógica de dash si la tienes
        // Por ahora solo un ejemplo de debug:
        Debug.Log("Dash pressed!");
    }

    private float GetHorizontalInput()
    {
        // 1) Si hay gamepad con axis asignado
        if (KeyBindingsManager.Instance != null && KeyBindingsManager.Instance.IsGamepadConnected())
        {
            string axis = KeyBindingsManager.Instance.GetGamepadAxisBinding("MoveHorizontal");
            Debug.Log($"[InputDebug] MoveHorizontal axis binding = '{axis}'");
            if (!string.IsNullOrEmpty(axis))
            {
                float val = Input.GetAxis(axis);
                Debug.Log($"[InputDebug] Input.GetAxis({axis}) = {val}");
                if (Mathf.Abs(val) > 0.12f) return val; // umbral
            }
        }

        // 2) fallback teclado con MoveLeft/MoveRight
        float h = 0f;
        if (KeyBindingsManager.Instance != null)
        {
            if (Input.GetKey(KeyBindingsManager.Instance.GetBinding("MoveRight", InputDeviceType.Keyboard))) h += 1f;
            if (Input.GetKey(KeyBindingsManager.Instance.GetBinding("MoveLeft", InputDeviceType.Keyboard))) h -= 1f;
        }
        return h;
    }
}
