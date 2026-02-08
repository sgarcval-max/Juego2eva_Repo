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
    [SerializeField] private string fireTriggerName = "fire";

    private float lastFireTime;

    private float xInput;
    private bool canJump = true;
    private int jumpCount = 0;
    public int maxJumps = 2;

    public bool IsDead => currentHealth <= 0;

    public static Player Instance; // <-- agregado


    private void OnEnable()
    {
        if (GameInput.Instance != null)
            GameInput.Instance.inputActions.Player.Enable();
    }

    protected override void Update()
    {
        base.Update();

        if (isGrounded)
            jumpCount = 0;

        HandleInput();
    }

    private void HandleInput()
    {
        if (KeyBindingsManager.Instance == null) return;

        xInput = GetHorizontalInput();

        KeyCode jumpKey = KeyBindingsManager.Instance.GetBinding("Jump", InputDeviceType.Keyboard);
        KeyCode attackKey = KeyBindingsManager.Instance.GetBinding("Attack", InputDeviceType.Keyboard);
        KeyCode fireKey = KeyBindingsManager.Instance.GetBinding("Fire", InputDeviceType.Keyboard);

        // ----------------- SALTO -----------------
        if (Input.GetKeyDown(jumpKey))
            TryToJump();

        // ----------------- ATAQUE NORMAL -----------------
        if (Input.GetKeyDown(attackKey))
        {
            if (AbilityManager.Instance != null && AbilityManager.Instance.meleeAttackUnlocked)
                HandleAttack();
        }

        // ----------------- ATAQUE CARGADO -----------------
        bool canUseChargedFire = AbilityManager.Instance != null &&
                                  AbilityManager.Instance.chargedFireUnlocked &&
                                  Time.time >= nextChargeTime;

        // Inicia carga solo si se presiona
        if (canUseChargedFire && Input.GetKeyDown(fireKey))
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        // Mientras se mantiene, aumenta chargeTimer
        if (isCharging && Input.GetKey(fireKey))
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0, maxChargeTime);
        }

        // Al soltar, dispara solo si se mantuvo y chargeTimer > 0
        if (isCharging && Input.GetKeyUp(fireKey))
        {
            if (chargeTimer > 0f)
            {
                ShootChargedFireball();
                nextChargeTime = Time.time + chargeCooldown;
            }
            isCharging = false;
            chargeTimer = 0f;
        }

        // ----------------- ATAQUE NORMAL -----------------
        // Solo dispara normal si NO estamos cargando
        if (!isCharging && Input.GetKeyDown(fireKey))
        {
            if (Time.time >= lastFireTime + fireCooldown)
            {
                lastFireTime = Time.time;
                anim.SetTrigger("fire");
                ShootFireball();
            }
        }

        HandleChargedFireInput();
    }
    // ----------------- ATAQUE CARGADO -----------------

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
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount = 1;
        }
        else if (AbilityManager.Instance != null &&
                 AbilityManager.Instance.doubleJumpUnlocked &&
                 jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
        }
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

    // -------------------- SISTEMA DE HABILIDADES --------------------

    public void UnlockAbility(AbilityType ability)
    {
        Debug.Log("Habilidad desbloqueada: " + ability);

        if (AbilityManager.Instance == null) return;

        switch (ability)
        {
            case AbilityType.DoubleJump:
                AbilityManager.Instance.doubleJumpUnlocked = true;
                break;

            case AbilityType.ChargedFire:
                AbilityManager.Instance.chargedFireUnlocked = true;
                break;

            case AbilityType.MeleeAttack:
                AbilityManager.Instance.meleeAttackUnlocked = true;
                break;
        }
    }

    // -------------------- ATAQUE CARGADO --------------------

    [Header("Charged Fire Attack")]
    [SerializeField] private GameObject chargedFireballPrefab;
    [SerializeField] private Transform chargedFirePoint;

    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float chargeCooldown = 3f;

    private float chargeTimer = 0f;
    private bool isCharging = false;
    private float nextChargeTime = 0f;

    private void HandleChargedFireInput()
    {
        if (AbilityManager.Instance == null) return;
        if (!AbilityManager.Instance.chargedFireUnlocked) return;
        if (Time.time < nextChargeTime) return;

        KeyCode fireKey = KeyBindingsManager.Instance.GetBinding("Fire", InputDeviceType.Keyboard);

        // Empieza a cargar
        if (Input.GetKeyDown(fireKey))
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        // Mantener tecla → cargar
        if (Input.GetKey(fireKey) && isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Clamp(chargeTimer, 0, maxChargeTime);
        }

        // Soltar tecla → dispara el ataque cargado
        if (Input.GetKeyUp(fireKey) && isCharging)
        {
            ShootChargedFireball();
            isCharging = false;
            nextChargeTime = Time.time + chargeCooldown;
        }
    }

    private void ShootChargedFireball()
    {
        if (chargedFireballPrefab == null || chargedFirePoint == null || chargeTimer <= 0f)
            return;

        GameObject fire = Instantiate(chargedFireballPrefab, chargedFirePoint.position, Quaternion.identity);
        Fireball fireballScript = fire.GetComponent<Fireball>();

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        fireballScript.SetDirection(dir);

        float damageMultiplier = 1f + (chargeTimer / maxChargeTime);
        fireballScript.damage = Mathf.RoundToInt(fireballScript.damage * damageMultiplier);
    }

    // -------------------- FIREBALL NORMAL --------------------

    public void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Falta asignar fireballPrefab o firePoint");
            return;
        }

        GameObject fb = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        Fireball fireballScript = fb.GetComponent<Fireball>();

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;

        fireballScript.SetDirection(dir);
    }

    public void FireAttack()
    {
        if (!canMove) return;

        EnableMovement(false);
        anim.SetTrigger(fireTriggerName);
    }

    public void TakeHealing(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);

        if (PlayerHealthManager.Instance != null)
            PlayerHealthManager.Instance.UpdateHealth(currentHealth);
    }
}