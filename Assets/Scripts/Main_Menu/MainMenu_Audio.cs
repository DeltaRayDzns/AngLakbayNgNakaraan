using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Audio : MonoBehaviour
{
    private bool hasPlayed = false;

    void Awake()
    {
        var existing = FindObjectsOfType<MainMenu_Audio>();
        if (existing.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (!hasPlayed && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
            hasPlayed = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopMusic();
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


}