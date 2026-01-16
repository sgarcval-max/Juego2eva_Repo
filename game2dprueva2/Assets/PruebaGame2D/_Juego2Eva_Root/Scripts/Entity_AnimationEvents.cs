using UnityEngine;

public class Entity_AnimationEvents : MonoBehaviour
{
    private Entity entity;

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }

    public void DamageTargets() => entity.DamageTargets();

    public void DisableMovementAndJump() => entity.EnableMovement(false);

    public void EnableMovementAndJump() => entity.EnableMovement(true);

    public void ShootFireball()
    {
        if (fireballPrefab != null && fireballSpawnPoint != null)
        {
            Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        }
        EnableMovementAndJump(); // permitir mover otra vez

        Player player = entity as Player;
        if (player != null)
            player.ShootFireball(); // Llama al método privado del Player

    }
}
