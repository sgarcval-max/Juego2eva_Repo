using UnityEngine;

public class Enemy : Entity
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 6f;
    private bool playerDetected;
    private bool playerInAttackRange;

    private Transform player;

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 3.5f;

    private void Start()
    {
        // Buscar al player automáticamente
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogError("[Enemy] No se encontró Player con tag 'Player'");
    }

    protected override void Update()
    {
        base.Update();
        HandleDetection();
        HandleAttack();
    }

    // ---------------- DETECTION ----------------
    private void HandleDetection()
    {
        if (player == null) return;

        // Detectar si el player está en rango de detección
        playerDetected = Vector2.Distance(transform.position, player.position) <= detectionRadius;

        // Detectar si está en rango de ataque
        playerInAttackRange = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            whatIsTarget
        );
    }

    // ---------------- ATTACK ----------------
    protected override void HandleAttack()
    {
        if (playerInAttackRange)
            anim.SetTrigger("attack");
    }

    // ---------------- MOVEMENT ----------------
    protected override void HandleMovement()
    {
        if (!playerDetected || !canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Determinar dirección hacia el player
        float directionToPlayer = (player.position.x - transform.position.x) > 0 ? 1f : -1f;

        // Si está mirando hacia el lado opuesto, hacer flip
        if ((directionToPlayer > 0 && !facingRight) || (directionToPlayer < 0 && facingRight))
            Flip();

        // Moverse hacia el player
        rb.linearVelocity = new Vector2(directionToPlayer * moveSpeed, rb.linearVelocity.y);
    }

    protected override void Die()
    {
        base.Die();
        UI.instance.AddKillCount();
    }

    // ---------------- DEBUG ----------------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}