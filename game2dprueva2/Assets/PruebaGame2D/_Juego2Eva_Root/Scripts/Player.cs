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

        // Movimiento horizontal
        if (Input.GetKey(KeyBindingsManager.Instance.GetBinding("MoveRight"))) xInput += 1;
        if (Input.GetKey(KeyBindingsManager.Instance.GetBinding("MoveLeft"))) xInput -= 1;

        // Saltar
        if (Input.GetKeyDown(KeyBindingsManager.Instance.GetBinding("Jump")))
            TryToJump();

        // Atacar
        if (Input.GetKeyDown(KeyBindingsManager.Instance.GetBinding("Attack")))
            HandleAttack();

        // Dash (si lo usas)
        if (Input.GetKeyDown(KeyBindingsManager.Instance.GetBinding("Dash")))
            HandleDash();
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
}
