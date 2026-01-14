using UnityEngine;

public class CameraFollow2D_Zone : MonoBehaviour
{
    [Header("Asignaciones")]
    public Player playerScript;       // Referencia al Player
    public Camera mainCamera;         // Main Camera
    public Vector3 offset = new Vector3(0, 3, -10); // Ajusta a tu gusto
    public float smoothSpeed = 5f;

    private bool stopCamera = false;
    private Vector3 holdPosition;

    void LateUpdate()
    {
        if (playerScript == null || mainCamera == null) return;

        // Si el jugador está muerto, la cámara se detiene en su posición actual
        if (playerScript.IsDead)
        {
            if (!stopCamera)
            {
                stopCamera = true;
                holdPosition = mainCamera.transform.position;
            }
        }

        Vector3 targetPosition = stopCamera ? holdPosition : playerScript.transform.position + offset;
        targetPosition.z = mainCamera.transform.position.z;

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stopCamera = true;
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
