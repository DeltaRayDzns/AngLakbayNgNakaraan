using UnityEngine;

public class Level3_Audio : MonoBehaviour
{
    void Start()
    {
        var mainMenuAudio = FindObjectOfType<MainMenu_Audio>();
        if (mainMenuAudio != null)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopMusic();

            Destroy(mainMenuAudio.gameObject);
        }
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayJapaneseColonization();
    }

    public void Level3ButtonSFX() 
    {
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlayInteractSFX(); 
        }
    }

    public void Level3SpeakingSFX() 
    {
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.PlaySpeakSFX();
        }
    }
}