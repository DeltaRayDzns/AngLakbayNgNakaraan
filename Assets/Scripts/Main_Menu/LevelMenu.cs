using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public void OpenLevel(int levelId)
    {
        if (levelId == 1) 
        {
            SceneManager.LoadScene("Intro"); 
        }
        else
        {
            string levelName = "Level" + levelId;
            SceneManager.LoadScene(levelName);
        }
    }
}