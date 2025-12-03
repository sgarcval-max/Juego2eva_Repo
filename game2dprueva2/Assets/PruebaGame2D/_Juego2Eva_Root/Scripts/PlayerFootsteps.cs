using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepClips; // array de pasos diferentes para variar

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0 || audioSource == null) return;
        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
    }
}