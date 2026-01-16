using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;

    [Header("Fireball Attack")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private string fireTriggerName = "fire"; // el trigger en el Animator

    private float lastFireTime;

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
        if (KeyBindingsManager.Instance == null) return;

        xInput = GetHorizontalInput();

        KeyCode jumpKey = KeyBindingsManager.Instance.GetBinding("Jump", InputDeviceType.Keyboard);
        KeyCode attackKey = KeyBindingsManager.Instance.GetBinding("Attack", InputDeviceType.Keyboard);
        KeyCode fireKey = KeyBindingsManager.Instance.GetBinding("Fire", InputDeviceType.Keyboard);

        // SALTO
        if (Input.GetKeyDown(jumpKey))
        {
            TryToJump();
        }

        // ATAQUE NORMAL
        if (Input.GetKeyDown(attackKey))
        {
            HandleAttack();
        }

        // ATAQUE FIREBALL
        if (Input.GetKeyDown(fireKey))
        {
            if (Time.time >= lastFireTime + fireCooldown)
            {
                lastFireTime = Time.time;

                anim.SetTrigger("fire");
                ShootFireball();
            }
        }
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
        if (KeyBindingsManager.Instance == null)
            return 0;

        KeyCode leftKey = KeyBindingsManager.Instance.GetBinding("MoveLeft", InputDeviceType.Keyboard);
        KeyCode rightKey = KeyBindingsManager.Instance.GetBinding("MoveRight", InputDeviceType.Keyboard);

        float input = 0;

        if (Input.GetKey(leftKey)) input -= 1;
        if (Input.GetKey(rightKey)) input += 1;

        return input;
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

    private void TryFire()
    {
        if (!canFireball) return;

        anim.SetTrigger("Fire");
        EnableMovement(false);
    }

    // Se llamará desde un EVENTO en la animación
    public void SpawnFireballFromAnimation()
    {
        FireballProjectile prefab = Resources.Load<FireballProjectile>("Fireball");

        if (prefab == null)
        {
            Debug.LogError("No se encontró el prefab Fireball en Resources");
            return;
        }

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;

        Vector3 spawnPos = transform.position + new Vector3(facingRight ? 1f : -1f, 0.5f, 0);

        Instantiate(prefab, spawnPos, Quaternion.identity)
            .Initialize(dir);
    }

    public void OnFireAnimationEnd()
    {
        EnableMovement(true);
    }

    public void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Falta asignar fireballPrefab o firePoint");
            return;
        }

        GameObject fb = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        Fireball fireballScript = fb.GetComponent<Fireball>();

        // Dirección según a dónde mira el player
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;

        fireballScript.SetDirection(dir);
    }

    public void FireAttack()
    {
        if (!canMove) return; // evita disparar mientras otra animación bloquea movimiento

        // desactivar movimiento
        EnableMovement(false);

        // disparar animación
        anim.SetTrigger(fireTriggerName);
    }


}