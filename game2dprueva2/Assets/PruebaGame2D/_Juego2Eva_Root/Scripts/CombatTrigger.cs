using UnityEngine;
using System.Collections;

public class CombatTrigger : MonoBehaviour
{
    [Header("Música de combate")]
    public AudioClip combatMusic; // música normal de combate
    public AudioClip bossMusic;   // música del boss (opcional)

    private bool inCombat = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !inCombat)
        {
            inCombat = true;
            StartCoroutine(ChangeMusicNextFrame());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && inCombat)
        {
            inCombat = false;
            StartCoroutine(ResetMusicNextFrame());
        }
    }

    private IEnumerator ChangeMusicNextFrame()
    {
        yield return null;
        if (bossMusic != null)
            AudioManager.Instance.EnterBoss();
        else
            AudioManager.Instance.EnterCombat();
    }

    private IEnumerator ResetMusicNextFrame()
    {
        yield return null;
        AudioManager.Instance.ExitCombat();
    }
}
