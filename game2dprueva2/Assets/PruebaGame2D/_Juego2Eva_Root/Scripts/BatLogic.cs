using UnityEngine;
using System.Collections;

public class BatLogic : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 7f; //numero de tiles para que detecte al jugador
    public string playerTag = "Player";
    public float reactionTime = 2f; //tiempo de espera antes de moverse

    [Header("Movimiento")]
    public float moveSpeed = 3f;

    private Transform player;
    private bool playerDetected = false;
    private bool canMove = false;

    [SerializeField] private Animator animator;

    void Update()
    {
        DetectPlayer();

        if (playerDetected && canMove)
        {
            FollowPlayer();
            animator.SetBool ("Detection", true);
        }
    }

    void DetectPlayer()
    {
        if (!playerDetected)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                float distance = Vector2.Distance(transform.position, playerObj.transform.position);

                if (distance <= detectionRadius)
                {
                    playerDetected = true;
                    player = playerObj.transform;
                    StartCoroutine(ReactionDelay()); //espera 2 segundos antes de atacar
                }
            }
        }
    }

    IEnumerator ReactionDelay()
    {
        //El enemigo detecta al jugador pero aún no se mueve
        yield return new WaitForSeconds(reactionTime);
        canMove = true;
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
