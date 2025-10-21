using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel_UI : MonoBehaviour
{
    [Header("Music Controls")]
    public Slider musicSlider;
    public Toggle musicToggle;

    [Header("SFX Controls")]
    public Slider sfxSlider;
    public Toggle sfxToggle;

    private void Start()
    {
        var audio = AudioManager.Instance;

        if (audio != null)
        {
            if (musicSlider != null)
                musicSlider.value = audio.musicSource.volume;

            if (sfxSlider != null)
                sfxSlider.value = audio.sfxSource.volume;

            if (musicToggle != null)
                musicToggle.isOn = !audio.musicSource.mute;

            if (sfxToggle != null)
                sfxToggle.isOn = !audio.sfxSource.mute;
        }

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggle);

        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(OnSFXToggle);
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnMusicToggle(bool isOn)
    {
        AudioManager.Instance.ToggleMusic(isOn);
    }

    private void OnSFXToggle(bool isOn)
    {
        AudioManager.Instance.ToggleSFX(isOn);
    }
}
