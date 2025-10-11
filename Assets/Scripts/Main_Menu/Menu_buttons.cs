using UnityEngine;

public class Menu_buttons : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
