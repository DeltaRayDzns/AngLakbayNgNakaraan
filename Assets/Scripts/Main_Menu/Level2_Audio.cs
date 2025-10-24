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
}
