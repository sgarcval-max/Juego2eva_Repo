using UnityEngine;

public class CameraFollow2D_Zone : MonoBehaviour
{
    [Header("Asignaciones")]
    public Transform player;       // Transform del jugador
    public Camera mainCamera;      // Main Camera
    public Vector3 offset = new Vector3(0, 3, -10); // Ajusta a tu gusto
    public float smoothSpeed = 5f;

    private bool stopCamera = false;
    private Vector3 holdPosition;

    void LateUpdate()
    {
        if (player == null || mainCamera == null) return;

        Vector3 targetPosition;

        if (stopCamera)
        {
            // La cámara se queda fija en holdPosition
            targetPosition = holdPosition;
        }
        else
        {
            // La cámara sigue al jugador + offset
            targetPosition = player.position + offset;
        }

        // Mantener la Z original de la cámara
        targetPosition.z = mainCamera.transform.position.z;

        // Suavizado de movimiento
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stopCamera = true;
            // Guardamos la posición actual de la cámara cuando entra
            holdPosition = mainCamera.transform.position;
            Debug.Log("Cámara congelada en: " + holdPosition);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stopCamera = false;
            Debug.Log("Cámara vuelve a seguir al jugador");
        }
    }
}