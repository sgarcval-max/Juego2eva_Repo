using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sfxMove;
    public AudioClip sfxConfirm;

    private float moveCooldown = 0.15f;
    private float lastMoveTime;

    // Llamar al navegar con teclado/mando
    public void TryPlayMove()
    {
        if (Time.time - lastMoveTime > moveCooldown)
        {
            if (audioSource != null && sfxMove != null)
                audioSource.PlayOneShot(sfxMove);
            lastMoveTime = Time.time;
        }
    }

    // Llamar al confirmar botón
    public void PlayConfirm()
    {
        if (audioSource != null && sfxConfirm != null)
            audioSource.PlayOneShot(sfxConfirm);
    }
}
