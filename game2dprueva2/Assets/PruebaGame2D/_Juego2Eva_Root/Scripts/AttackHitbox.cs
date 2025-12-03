using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackHitbox : MonoBehaviour
{
    public PlayerAttackSounds owner;  // referencia asignada en Inspector

    void Reset()
    {
        // Asegúrate de que el collider está en modo Trigger por defecto
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (owner == null) return;
        // Reporta al owner que hay un hit
        owner.ReportHit(other.gameObject, other);
    }
}
