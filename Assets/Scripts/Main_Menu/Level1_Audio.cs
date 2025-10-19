using UnityEngine;

public class SceneAudio_Level1 : MonoBehaviour
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
            AudioManager.Instance.PlaySpanishColonization();
    }
}