using UnityEngine;

public class HeartFlyToUI : MonoBehaviour
{
    public float speed = 6f;

    private Transform target;

    private void Start()
    {
        target = PlayerHealthManager.Instance.heartTarget;
        // ← pon aquí el transform del icono de vida
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            Destroy(gameObject);
    }
}
