using System.Collections;
using UnityEngine;

public class PlayerAttackSounds : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;               // Asignar en Inspector (AudioSource en el Player)
    public AudioClip[] swingClips;                // sonidos de "swish" cuando no golpea nada
    public AudioClip[] hitClips;                  // sonidos cuando golpea a un enemigo

    [Header("Hitbox")]
    public AttackHitbox attackHitbox;             // referencia al objeto hitbox (desactivado por defecto)
    public float hitWindow = 0.12f;               // tiempo (s) que la hitbox está activa
    public LayerMask enemyLayer;                  // capa(s) que consideras enemigos (mejor que tag)

    [Header("Variación")]
    public float pitchMin = 0.95f;
    public float pitchMax = 1.05f;
    public float volume = 1f;

    // estado interno
    private bool hitDetected;
    private GameObject hitEnemy;

    void Reset()
    {
        // Si no hay audioSource asignado, intenta encontrar uno en el mismo GameObject
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("PlayerAttackSounds: No AudioSource asignado en " + name);
        }
        if (attackHitbox == null)
        {
            Debug.LogWarning("PlayerAttackSounds: No AttackHitbox asignado en " + name);
        }
    }

    // Este método lo llamas desde el Animation Event (nombre exacto)
    public void StartAttackWindow()
    {
        StartCoroutine(AttackWindowCoroutine());
    }

    IEnumerator AttackWindowCoroutine()
    {
        hitDetected = false;
        hitEnemy = null;

        // Activa la hitbox (si existe)
        if (attackHitbox != null)
            attackHitbox.gameObject.SetActive(true);

        // Espera la ventana
        float elapsed = 0f;
        while (elapsed < hitWindow)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Desactiva la hitbox
        if (attackHitbox != null)
            attackHitbox.gameObject.SetActive(false);

        // Reproduce el sonido adecuado
        if (hitDetected)
        {
            PlayRandomClip(hitClips);
            // aquí podrías ejecutar lógica extra: aplicar daño, efectos, etc.
        }
        else
        {
            PlayRandomClip(swingClips);
        }
    }

    // Llamado por AttackHitbox cuando detecta colisión
    public void ReportHit(GameObject enemy, Collider2D col)
    {
        // Verifica layer si usas LayerMask (opcional)
        if (((1 << enemy.layer) & enemyLayer) == 0)
        {
            // no es enemigo según layer
            return;
        }

        hitDetected = true;
        hitEnemy = enemy;
        // Puedes almacenar más info del contacto si quieres (col, punto, etc.)
    }

    void PlayRandomClip(AudioClip[] clips)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;

        int i = Random.Range(0, clips.Length);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(clips[i], volume);
    }
}