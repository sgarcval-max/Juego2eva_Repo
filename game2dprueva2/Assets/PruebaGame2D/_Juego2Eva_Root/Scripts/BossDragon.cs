using UnityEngine;
using System.Collections;

public class BossDragon : Entity
{
    public enum BossPhase { Phase1, Phase2, Phase3 }

    [Header("Boss Config")]
    public BossPhase currentPhase = BossPhase.Phase1;

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

    private float nextFireTime;
    private float nextShockwaveTime;

    private bool isTransitioning = false;

    [Header("Activation")]
    public bool fightStarted = false;
    public float activationDistance = 5f; // distancia m�nima al jugador

    [Header("Fase 3 - Seguimiento")]
    public float followDistance = 3f; // distancia mínima que mantiene respecto al jugador
    public float stopDistance = 1f;   // distancia a la que deja de moverse

    private void RegeneratePlayerHealth()
    {
        if (PlayerHealthManager.Instance == null) return;

        Player player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        if (player == null) return;

        int maxPlayerHealth = player.MaxHealth; // vida máxima
        int currentPlayerHealth = player.CurrentHealth;

        int lostHealth = maxPlayerHealth - currentPlayerHealth;

        if (lostHealth > 0)
        {
            int regenAmount = Mathf.Min(2, lostHealth); // regenera 2 como máximo o lo que haya perdido
            player.SetHealth(currentPlayerHealth + regenAmount);
            PlayerHealthManager.Instance.UpdateHealth(player.CurrentHealth);

            Debug.Log($"Jugador regeneró {regenAmount} de vida al iniciar nueva fase");
        }
    }

    void CheckActivation()
    {
        if (fightStarted) return;

        if (player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= activationDistance)
        {
            StartCoroutine(StartPhase(1));
            fightStarted = true;
        }
    }

    IEnumerator StartPhase(int phaseNumber)
    {
        isTransitioning = true;

        // Mostrar texto
        BossUIManager.Instance.ShowPhaseText(phaseNumber);

        yield return new WaitForSeconds(2f);

        // Comenzar fase
        currentPhase = (BossPhase)(phaseNumber - 1); // Phase1 = 0
        currentHealth = GetMaxHealthForPhase();

        BossHealthBar.Instance.UpdateHealth(currentHealth, GetMaxHealthForPhase());

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
            case BossPhase.Phase1:
                Phase1Logic();
                break;
            case BossPhase.Phase2:
                Phase2Logic();
                break;
            case BossPhase.Phase3:
                Phase3Logic();
                break;
        }
    }

    void Phase1Logic()
    {
        TryShootFireball();
    }

    void Phase2Logic()
    {
        TryShootFireball();
        TryShockwave();
    }

    void Phase3Logic()
    {
        TryShootFireball();
        TryShockwave();

        MoveTowardsPlayer();
    }

    void TryShootFireball()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        }
    }

    void TryShockwave()
    {
        if (Time.time >= nextShockwaveTime && shockwavePrefab != null && shockwavePoint != null)
        {
            nextShockwaveTime = Time.time + shockwaveRate;

            // Instancia la onda en el punto
            GameObject sw = Instantiate(shockwavePrefab, shockwavePoint.position, Quaternion.identity);

            // Opcional: si quieres que vaya hacia el jugador
            Vector2 dir = (player.position - shockwavePoint.position).normalized;
            Shockwave shockScript = sw.GetComponent<Shockwave>();
            if (shockScript != null)
            {
                shockScript.transform.right = dir; // orientar la onda hacia el jugador
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        // Calculamos la distancia horizontal entre dragón y jugador
        float distanceX = player.position.x - transform.position.x;

        // Solo nos movemos si estamos fuera del rango deseado
        if (Mathf.Abs(distanceX) > followDistance)
        {
            float directionX = Mathf.Sign(distanceX); // -1 o 1 según la dirección
            transform.position += new Vector3(directionX * moveSpeed * Time.deltaTime, 0, 0);

            // Flip visual
            if (directionX > 0 && !facingRight) Flip();
            else if (directionX < 0 && facingRight) Flip();
        }
    }

    public override void TakeDamage(int amount = 1)
    {
        base.TakeDamage(amount);

        BossHealthBar.Instance?.UpdateHealth(currentHealth, GetMaxHealthForPhase());
    }

    protected override void Die()
    {
        if (isTransitioning) return;

        StartCoroutine(NextPhaseRoutine());
    }

    IEnumerator NextPhaseRoutine()
    {
        isTransitioning = true;

        yield return new WaitForSeconds(2f);

        // Regenerar vida del jugador al terminar la fase
        RegeneratePlayerHealth();

        if (currentPhase == BossPhase.Phase1)
        {
            currentPhase = BossPhase.Phase2;
            currentHealth = phase2Health;
            BossUIManager.Instance.ShowPhaseText(2);
        }
        else if (currentPhase == BossPhase.Phase2)
        {
            currentPhase = BossPhase.Phase3;
            currentHealth = phase3Health;
            BossUIManager.Instance.ShowPhaseText(3);
        }
        else
        {
            // BOSS DERROTADO FINALMENTE
            Destroy(gameObject);
            BossUIManager.Instance.ShowBossDefeated();
            yield break;
        }

        BossHealthBar.Instance.UpdateHealth(currentHealth, GetMaxHealthForPhase());

        isTransitioning = false;
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
