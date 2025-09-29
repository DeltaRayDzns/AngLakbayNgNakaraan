using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; 
using TMPro;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public GameObject[] SlidesTotal;
    public TextMeshProUGUI ReminderText;
    int index = 0;
    int previousindex = 0;
    private float timer =0;  

    void Start()
    {
        ReminderText.text = "";
        SlidesTotal[index].SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timer = 0;
            ReminderText.text = "";
            nextSlide();
        }

        Reminder();
    }

    public void Reminder()
    {
        if (timer >= 15f)
        {
            ReminderText.text = "Press Space to continue...";
            Debug.Log("Reminder shown");
        }
    }

    public void nextSlide()
    {

        previousindex = index;

        index++;

        if (index >= 7)
        {
            SceneManager.LoadScene("Level1");
            return;
        }


        SlidesTotal[previousindex].SetActive(false);
        SlidesTotal[index].SetActive(true);
    }
}