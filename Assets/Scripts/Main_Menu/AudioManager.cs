using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenu;
    public AudioClip spanishColonization;
    public AudioClip americanColonization;
    public AudioClip japaneseColonization;
    public AudioClip endMusic;

    [Header("SFX Clips")]
    public AudioClip damageSFX;
    public AudioClip deathSFX;
    public AudioClip interactSFX;
    public AudioClip SpeakSFX;
    public AudioClip WalkingSFX; 
	public AudioClip levelVictory; 

    [Header("UI References")]
    public GameObject optionsPanel;
    private CanvasGroup optionsCanvasGroup;

    public bool musicOn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (optionsPanel != null)
        {
            DontDestroyOnLoad(optionsPanel);
            SetupOptionsCanvasGroup();
        }
    }

    private void SetupOptionsCanvasGroup()
    {
        optionsCanvasGroup = optionsPanel.GetComponent<CanvasGroup>();
        if (optionsCanvasGroup == null)
            optionsCanvasGroup = optionsPanel.AddComponent<CanvasGroup>();

        optionsCanvasGroup.alpha = 0;
        optionsCanvasGroup.interactable = false;
        optionsCanvasGroup.blocksRaycasts = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicSource == null || sfxSource == null)
        {
            var panel = FindObjectOfType<OptionsPanel_persistence>();
            if (panel != null)
                panel.ReconnectAudioSources();
        }
    }

    public void PlayMainMenuMusic() => PlayMusic(mainMenu);
    public void PlaySpanishColonization() => PlayMusic(spanishColonization);
    public void PlayAmericanColonization() => PlayMusic(americanColonization);
    public void PlayJapaneseColonization() => PlayMusic(japaneseColonization);
    public void PlayEndMusic() => PlayMusic(endMusic);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
        {
            Debug.LogWarning("[AudioManager] Missing clip or MusicSource!");
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();

        Debug.Log($"[AudioManager] Playing music: {clip.name}");
    }

    public void PlayDamageSFX() => PlaySFX(damageSFX);
    public void PlayDeathSFX() => PlaySFX(deathSFX);
    public void PlayInteractSFX() => PlaySFX(interactSFX);
    public void PlaySpeakSFX() => PlaySFX(SpeakSFX);
	public void PlayVictorySFX() => PlaySFX(levelVictory);
	public void PlayWalkingSFX() => PlaySFX(WalkingSFX);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
    }

    public void ToggleMusic(bool isOn)
    {
        if (musicSource != null)
            musicSource.mute = !isOn;
    }

    public void ToggleSFX(bool isOn)
    {
        if (sfxSource != null)
            sfxSource.mute = !isOn;
    }
	
		public void FadeOutMusic(float fadeTime = 1.5f)
	{
    	StartCoroutine(FadeOutCoroutine(fadeTime));
	}

	private System.Collections.IEnumerator FadeOutCoroutine(float fadeTime)
	{
    	if (musicSource == null)
        yield break;

    	float startVolume = musicSource.volume;

    	for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
    	{
        	musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
        	yield return null;
    	}

    	musicSource.Stop();
    	musicSource.volume = startVolume; 
	}

	
    public void StopMusic()
    {
        musicOn = false;
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    public void OpenOptions()
    {
        if (optionsCanvasGroup == null && optionsPanel != null)
            SetupOptionsCanvasGroup();

        if (optionsCanvasGroup != null)
        {
            optionsCanvasGroup.alpha = 1;
            optionsCanvasGroup.interactable = true;
            optionsCanvasGroup.blocksRaycasts = true;
        }
        else if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        if (optionsCanvasGroup == null && optionsPanel != null)
            SetupOptionsCanvasGroup();

        if (optionsCanvasGroup != null)
        {
            optionsCanvasGroup.alpha = 0;
            optionsCanvasGroup.interactable = false;
            optionsCanvasGroup.blocksRaycasts = false;
        }
    }
}
