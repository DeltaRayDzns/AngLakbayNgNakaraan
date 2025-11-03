using UnityEngine;

public class Epilogue_audio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEndMusic();
    }
}
