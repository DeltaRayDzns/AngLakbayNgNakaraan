using UnityEngine;

public class Level2_Audio : MonoBehaviour
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
            AudioManager.Instance.PlayAmericanColonization();
    }

	public void Level2ButtonSFX() 
	{
		if (AudioManager.Instance != null) 
		{
			AudioManager.Instance.PlayInteractSFX(); 
		}
	}

	public void Level2SpeakingSFX() 
	{
		if (AudioManager.Instance != null) 
		{
			AudioManager.Instance.PlaySpeakSFX();
		}
	}
}
