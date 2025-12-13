using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";

    private void Awake()
    {
        // Singleton
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

        // Cargar valores guardados
        float musicVal = PlayerPrefs.GetFloat(MUSIC_PARAM, 1f);
        float sfxVal = PlayerPrefs.GetFloat(SFX_PARAM, 1f);

        if (musicSlider != null) musicSlider.value = musicVal;
        if (sfxSlider != null) sfxSlider.value = sfxVal;

        ApplyMusicVolume(musicVal);
        ApplySFXVolume(sfxVal);

        // Suscribir eventos
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(ApplySFXVolume);
    }

    public void ApplyMusicVolume(float value)
    {
        // Clamp para evitar log10(0)
        float vol = Mathf.Clamp(value, 0.0001f, 1f);
        audioMixer.SetFloat(MUSIC_PARAM, Mathf.Log10(vol) * 20f);
        PlayerPrefs.SetFloat(MUSIC_PARAM, value);
        PlayerPrefs.Save();
    }

    public void ApplySFXVolume(float value)
    {
        float vol = Mathf.Clamp(value, 0.0001f, 1f);
        audioMixer.SetFloat(SFX_PARAM, Mathf.Log10(vol) * 20f);
        PlayerPrefs.SetFloat(SFX_PARAM, value);
        PlayerPrefs.Save();
    }
}