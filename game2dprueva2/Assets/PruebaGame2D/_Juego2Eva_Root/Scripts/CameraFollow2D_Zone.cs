using UnityEngine;

public class CameraFollow2D_Zone : MonoBehaviour
{
    [Header("Asignaciones")]
    public Transform player;
    public Camera mainCamera;
    public Vector3 offset = new Vector3(0, 3, -10);
    public float smoothSpeed = 5f;

    private bool stopCamera = false;      // si la cámara está congelada por la zona
    private Vector3 holdPosition;
    private bool stopCameraPermanently = false; // ❌ nueva variable para muerte

    void LateUpdate()
    {
        if (player == null || mainCamera == null) return;

        Vector3 targetPosition;

        if (stopCamera || stopCameraPermanently)
        {
            // La cámara se queda fija
            targetPosition = holdPosition;
        }
        else
        {
            targetPosition = player.position + offset;
        }

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

    // ✅ Nuevo método que llama Player al morir
    public void FreezeCameraPermanently()
    {
        stopCameraPermanently = true;
        holdPosition = mainCamera.transform.position;
        Debug.Log("Cámara congelada permanentemente por muerte del jugador");
    }

    // ✅ Para reiniciar la partida y desbloquear la cámara
    public void ResetCamera()
    {
        stopCameraPermanently = false;
    }
}
