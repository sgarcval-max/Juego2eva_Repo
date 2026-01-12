using UnityEngine;
using System.Collections;

public class PlayerIntroCutscene2D : MonoBehaviour
{
    [Header("Player Settings")]
    public Player playerScript;         // Tu script Player
    public Transform targetPoint;       // Punto donde queremos que se detenga el Player
    public float moveSpeed = 3f;        // Velocidad del Player

    [Header("Cutscene Settings")]
    public float stopDistance = 0.1f;   // Distancia mínima para considerar que llegó

    private bool cutsceneActive = true;

    private void Start()
    {
        if (playerScript == null)
            playerScript = FindObjectOfType<Player>();

        if (playerScript != null && targetPoint != null)
        {
            playerScript.EnableMovement(false); // bloqueamos control del jugador
            StartCoroutine(RunIntroCutscene());
        }
        else
        {
            Debug.LogError("PlayerIntroCutscene2D: Faltan referencias.");
            cutsceneActive = false;
        }
    }

    private IEnumerator RunIntroCutscene()
    {
        while (cutsceneActive)
        {
            if (playerScript == null || targetPoint == null) yield break;

            // dirección hacia el punto
            Vector2 direction = (targetPoint.position - playerScript.transform.position).normalized;

            // mover al player
            playerScript.PlayCutsceneMovement(direction * moveSpeed);

            // comprobamos distancia
            float distance = Vector2.Distance(playerScript.transform.position, targetPoint.position);
            if (distance <= stopDistance)
            {
                cutsceneActive = false;
                playerScript.PlayCutsceneMovement(Vector2.zero); // detener al player
                playerScript.EnableMovement(true);               // dar control al jugador
            }

            yield return null;
        }
    }
}
