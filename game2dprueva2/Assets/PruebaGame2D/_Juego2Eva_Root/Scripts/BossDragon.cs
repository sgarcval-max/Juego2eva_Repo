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
    public float activationDistance = 5f;

    [Header("Fase 3 - Seguimiento")]
    public float followDistance = 3f;
    public float stopDistance = 1f;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = phase1Health;

        // Ocultar barra al empezar
        if (BossHealthBar.Instance != null)
            BossHealthBar.Instance.gameObject.SetActive(false);
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

    void CheckActivation()
    {
        if (fightStarted) return;

        if (player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= activationDistance)
        {
            fightStarted = true;

            // Mostrar barra de vida al empezar
            BossHealthBar.Instance?.gameObject.SetActive(true);

            StartCoroutine(StartPhase(1));
        }
    }

    IEnumerator StartPhase(int phaseNumber)
    {
        isTransitioning = true;

        BossUIManager.Instance.ShowPhaseText(phaseNumber);

        yield return new WaitForSeconds(2f);

        currentPhase = (BossPhase)(phaseNumber - 1);
        currentHealth = GetMaxHealthForPhase();

        BossHealthBar.Instance.UpdateHealth(currentHealth, GetMaxHealthForPhase());

        isTransitioning = false;
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

            GameObject sw = Instantiate(shockwavePrefab, shockwavePoint.position, Quaternion.identity);

            Vector2 dir = (player.position - shockwavePoint.position).normalized;
            Shockwave shockScript = sw.GetComponent<Shockwave>();
            if (shockScript != null)
            {
                shockScript.transform.right = dir;
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return;

        float distanceX = player.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) > followDistance)
        {
            float directionX = Mathf.Sign(distanceX);
            transform.position += new Vector3(directionX * moveSpeed * Time.deltaTime, 0, 0);

            if (directionX > 0 && !facingRight) Flip();
            else if (directionX < 0 && facingRight) Flip();
        }
    }

    public override void TakeDamage(int amount = 1)
    {
        if (isTransitioning) return;   // 🔒 Inmortal durante textos

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

        RegeneratePlayerHealth();

        int oldMaxHealth = GetMaxHealthForPhase();

        if (currentPhase == BossPhase.Phase1)
        {
            currentPhase = BossPhase.Phase2;
            BossUIManager.Instance.ShowPhaseText(2);

            yield return StartCoroutine(AnimateHealthBar(oldMaxHealth, phase2Health));

            currentHealth = phase2Health;
        }
        else if (currentPhase == BossPhase.Phase2)
        {
            currentPhase = BossPhase.Phase3;
            BossUIManager.Instance.ShowPhaseText(3);

            yield return StartCoroutine(AnimateHealthBar(oldMaxHealth, phase3Health));

            currentHealth = phase3Health;
        }
        else
        {
            Destroy(gameObject);
            BossUIManager.Instance.ShowBossDefeated();

            // Ocultar barra al final
            BossHealthBar.Instance?.gameObject.SetActive(false);

            yield break;
        }

        BossHealthBar.Instance.UpdateHealth(currentHealth, GetMaxHealthForPhase());

        isTransitioning = false;
    }

    IEnumerator AnimateHealthBar(int from, int to)
    {
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int current = (int)Mathf.Lerp(from, to, elapsed / duration);

            BossHealthBar.Instance.UpdateHealth(current, to);

            yield return null;
        }

        BossHealthBar.Instance.UpdateHealth(to, to);
    }

    private void RegeneratePlayerHealth()
    {
        if (PlayerHealthManager.Instance == null) return;

        Player player = GameObject.FindWithTag("Player")?.GetComponent<Player>();
        if (player == null) return;

        int maxPlayerHealth = player.MaxHealth;
        int currentPlayerHealth = player.CurrentHealth;

        int lostHealth = maxPlayerHealth - currentPlayerHealth;

        if (lostHealth > 0)
        {
            int regenAmount = Mathf.Min(2, lostHealth);
            player.SetHealth(currentPlayerHealth + regenAmount);
            PlayerHealthManager.Instance.UpdateHealth(player.CurrentHealth);

            Debug.Log($"Jugador regeneró {regenAmount} de vida al iniciar nueva fase");
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
