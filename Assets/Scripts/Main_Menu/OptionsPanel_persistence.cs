using UnityEngine;

public class OptionsPanel_persistence : MonoBehaviour
{
    private static OptionsPanel_persistence instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ReconnectAudioSources(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReconnectAudioSources()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[OptionsPanel] No AudioManager found!");
            return;
        }

        var musicSource = transform.Find("Music")?.GetComponent<AudioSource>();
        var sfxSource = transform.Find("SFX")?.GetComponent<AudioSource>();

        if (musicSource != null)
            AudioManager.Instance.musicSource = musicSource;

        if (sfxSource != null)
            AudioManager.Instance.sfxSource = sfxSource;

        Debug.Log("[OptionsPanel] Reconnected Audio Sources to AudioManager.");
    }
}
