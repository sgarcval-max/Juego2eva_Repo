using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;       // El jugador

    [Header("Follow Settings")]
    public float smoothSpeed = 5f; // Suavidad del movimiento
    public Vector3 offset;         // Posición relativa

    [HideInInspector]
    public bool stopFollowing = false; // Control para parar/seguir

    void LateUpdate()
    {
        // Si la cámara está parada o no hay objetivo, no hace nada
        if (stopFollowing || target == null)
            return;

        // Posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Movimiento suave
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Mover cámara
        transform.position = smoothed;
    }
}