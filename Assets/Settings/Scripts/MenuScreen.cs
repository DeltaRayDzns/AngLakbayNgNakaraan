using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
public class MenuScreen : MonoBehaviour
{
   public void OnPlayButton()
    {
        SceneManager.LoadScene("MiniGame");
    }
    public void OnQuitButton()
    {
        Application.Quit();
    }
}
