using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;      // música normal del nivel
    public AudioClip combatMusic;        // enemigos normales
    public AudioClip bossMusic;          // boss

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Crossfade settings")]
    public float fadeDuration = 1f;

    private bool inCombat = false;
    private bool inBossFight = false;

    // ---------------- Guardar tiempo solo para combat y boss ----------------
    private Dictionary<AudioClip, float> clipTimes = new Dictionary<AudioClip, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadVolumeSettings();
        PreloadAudioClips();
        PlaySceneMusic();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void PreloadAudioClips()
    {
        menuMusic?.LoadAudioData();
        gameplayMusic?.LoadAudioData();
        combatMusic?.LoadAudioData();
        bossMusic?.LoadAudioData();

        // Inicializar tiempos solo para combate y boss
        clipTimes[combatMusic] = 0f;
        clipTimes[bossMusic] = 0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        inCombat = false;
        inBossFight = false;
        PlaySceneMusic();
    }

    public void PlaySceneMusic()
    {
        if (SceneManager.GetActiveScene().name.Contains("Menu"))
        {
            if (menuMusic != null)
                PlayMusic(menuMusic);
        }
        else
        {
            if (gameplayMusic != null)
                PlayMusic(gameplayMusic); // siempre empieza desde 0
        }
    }

    // -------------------- MÚSICA --------------------
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    // -------------------- SFX --------------------
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSource != null) musicSource.volume = musicVolume;
    }

    // -------------------- FADE MÚSICA --------------------
    public void FadeMusic(float targetVolume, float duration)
    {
        if (musicSource != null)
            StartCoroutine(FadeCoroutine(targetVolume, duration));
    }

    private IEnumerator FadeCoroutine(float target, float duration)
    {
        float start = musicSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        musicSource.volume = target;
    }

    // ==================== CROSSFADE SOLO PARA COMBATE Y BOSS ====================
    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip)
            yield break;

        // Guardar tiempo solo si es combat o boss
        if ((musicSource.clip == combatMusic || musicSource.clip == bossMusic) &&
            musicSource.isPlaying && musicSource.clip != null)
        {
            clipTimes[musicSource.clip] = musicSource.time;
        }

        float t = 0f;
        float startVolume = musicSource.volume;

        // Fade out
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        // Cambiar clip
        musicSource.clip = newClip;

        // Reanudar desde el tiempo guardado solo si es combat o boss
        if (newClip == combatMusic || newClip == bossMusic)
        {
            if (clipTimes.ContainsKey(newClip))
                musicSource.time = clipTimes[newClip];
            else
                musicSource.time = 0f;
        }
        else
        {
            musicSource.time = 0f; // música normal empieza desde 0
        }

        musicSource.Play();

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0, musicVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    // ==================== FUNCIONES PARA COMBATE ====================
    public void EnterCombat()
    {
        if (inBossFight) return;
        inCombat = true;
        StartCoroutine(CrossfadeMusic(combatMusic));
    }

    public void ExitCombat()
    {
        if (inBossFight) return;
        inCombat = false;
        StartCoroutine(CrossfadeMusic(gameplayMusic));
    }

    public void EnterBoss()
    {
        inBossFight = true;
        StartCoroutine(CrossfadeMusic(bossMusic));
    }

    public void ExitBoss()
    {
        inBossFight = false;
        inCombat = false;
        StartCoroutine(CrossfadeMusic(gameplayMusic));
    }
}