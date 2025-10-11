using UnityEngine;
using UnityEngine.SceneManagement; 

public class ToMainMenu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
        Time.timeScale = 1f;
    }

}
