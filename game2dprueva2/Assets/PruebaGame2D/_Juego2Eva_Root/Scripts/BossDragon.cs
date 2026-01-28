using UnityEngine;
using System.Collections;

public class BossDragon : Entity
{
    public enum BossPhase { Phase1, Phase2, Phase3 }

    [Header("Boss Config")]
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Animator")]
    public Animator animator;

    public int phase1Health = 10;
    public int phase2Health = 15;
    public int phase3Health = 20;

    [Header("Ataques")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    public GameObject shockwavePrefab;
    public Transform shockwavePoint;

    public float fireRate = 2f;
    public float shockwaveRate = 4f;

    [Header("Movimiento Fase 3")]
    public float moveSpeed = 3f;
    public Transform player;

    [Header("Movimiento Fase 3")]
    public float followDistance = 3f; // distancia mínima que mantiene respecto al jugador
    public float stopDistance = 1f;   // opcional: distancia a la que deja de moverse

    private float nextFireTime;
    private float nextShockwaveTime;

    private bool isTransitioning = false;
    private bool fightStarted = false;
    private bool canTakeDamage = true;

    public float activationDistance = 5f;

    void CheckActivation()
    {
        if (fightStarted) return;
        if (player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= activationDistance)
        {
            fightStarted = true;

            // MOSTRAR BARRA AL INICIAR EL COMBATE
            if (BossHealthBar.Instance != null)
                BossHealthBar.Instance.Show();

            StartCoroutine(StartPhase(1));
        }
    }

    IEnumerator StartPhase(int phaseNumber)
    {
        isTransitioning = true;
        canTakeDamage = false;

        BossUIManager.Instance.ShowPhaseTextFade(phaseNumber);

        yield return new WaitForSeconds(2f);

        currentPhase = (BossPhase)(phaseNumber - 1);
        currentHealth = GetMaxHealthForPhase();

        BossHealthBar.Instance.UpdateHealth(currentHealth, GetMaxHealthForPhase());

        canTakeDamage = true;
        isTransitioning = false;
    }

    protected override void Awake()
    {
        base.Awake();
        currentHealth = phase1Health;
    }

    protected override void Update()
    {
        base.Update();

        if (!fightStarted)
        {
            CheckActivation();
            return;
        }

        if (isTransitioning) return;

        switch (currentPhase)
        {
            case BossPhase.Phase1: Phase1Logic(); break;
            case BossPhase.Phase2: Phase2Logic(); break;
            case BossPhase.Phase3: Phase3Logic(); break;
        }
    }

    void Phase1Logic()
    {
        FacePlayer();
        TryShootFireball();
    }

    void Phase2Logic()
    {
        FacePlayer();
        TryShootFireball();
        TryShockwave();
    }
    void Phase3Logic()
    {
        TryShootFireball();
        TryShockwave();

        MoveTowardsPlayer(); // Esto hace que el dragón siga al jugador
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        float distanceX = player.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) > followDistance)
        {
            float directionX = Mathf.Sign(distanceX);
            transform.position += new Vector3(directionX * moveSpeed * Time.deltaTime, 0, 0);

            // Flip corregido
            if (directionX > 0 && facingRight) Flip();
            else if (directionX < 0 && !facingRight) Flip();
        }

        // Animación de movimiento
        if (animator != null)
            animator.SetBool("isMoving", Mathf.Abs(distanceX) > followDistance);
    }

    void FacePlayer()
    {
        if (player == null) return;

        float distanceX = player.position.x - transform.position.x;

        if (distanceX > 0 && facingRight)
            Flip();
        else if (distanceX < 0 && !facingRight)
            Flip();
    }

    void TryShootFireball()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            if (animator != null)
                animator.SetTrigger("attackFireball");
        }
    }

    void TryShockwave()
    {
        if (Time.time >= nextShockwaveTime)
        {
            nextShockwaveTime = Time.time + shockwaveRate;
            Instantiate(shockwavePrefab, shockwavePoint.position, Quaternion.identity);

            if (animator != null)
                animator.SetTrigger("attackShockwave");
        }
    }

    public override void TakeDamage(int amount = 1)
    {
        if (!canTakeDamage) return;

        base.TakeDamage(amount);
        BossHealthBar.Instance.UpdateHealth(currentHealth, GetMaxHealthForPhase());

        if (animator != null)
            animator.SetTrigger("hit");
    }

    protected override void Die()
    {
        if (isTransitioning) return;

        StartCoroutine(NextPhaseRoutine());
    }

    IEnumerator NextPhaseRoutine()
    {
        isTransitioning = true;
        canTakeDamage = false;

        yield return new WaitForSeconds(2f);

        // Regenerar al jugador hasta 2 puntos
        RegeneratePlayerHealth();

        if (currentPhase == BossPhase.Phase1)
        {
            currentPhase = BossPhase.Phase2;
            currentHealth = phase2Health;

            BossUIManager.Instance.ShowPhaseTextFade(2);

            // 🔥 ANIMAR RELLENO DE VIDA
            BossHealthBar.Instance.AnimateRefill(phase2Health);
        }
        else if (currentPhase == BossPhase.Phase2)
        {
            currentPhase = BossPhase.Phase3;
            currentHealth = phase3Health;

            BossUIManager.Instance.ShowPhaseTextFade(3);

            // 🔥 ANIMAR RELLENO DE VIDA
            BossHealthBar.Instance.AnimateRefill(phase3Health);
        }
        else
        {
            // FINAL DEL BOSS
            BossUIManager.Instance.ShowBossDefeated();

            // OCULTAR BARRA AL MORIR
            BossHealthBar.Instance.Hide();

            Destroy(gameObject);
            yield break;
        }

        // Esperar un poco a que el jugador vea el texto y la barra llenarse
        yield return new WaitForSeconds(2.5f);

        canTakeDamage = true;
        isTransitioning = false;
    }

    private void RegeneratePlayerHealth()
    {
        if (PlayerHealthManager.Instance == null) return;

        Player player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        if (player == null) return;

        int lostHealth = player.MaxHealth - player.CurrentHealth;
        if (lostHealth > 0)
        {
            int regen = Mathf.Min(2, lostHealth);
            player.SetHealth(player.CurrentHealth + regen);
            PlayerHealthManager.Instance.UpdateHealth(player.CurrentHealth);
        }
    }

    int GetMaxHealthForPhase()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1: return phase1Health;
            case BossPhase.Phase2: return phase2Health;
            case BossPhase.Phase3: return phase3Health;
        }
        return phase1Health;
    }
}

