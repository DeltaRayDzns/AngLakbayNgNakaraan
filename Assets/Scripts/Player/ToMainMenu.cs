using UnityEngine;
using UnityEngine.SceneManagement; 

public class ToMainMenu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
        Time.timeScale = 1f;
    }

    public void Level1_NextLevel()
    {
        SceneManager.LoadScene("Level2");
        Time.timeScale = 1f; 
    }

    public void Level2_NextLevel()
    {
        SceneManager.LoadScene("Level3");
        Time.timeScale = 1f; 
    }

    public void FinishGame()
    {
        SceneManager.LoadScene("Epilouge");
        Time.timeScale = 1f; 
    }
}

