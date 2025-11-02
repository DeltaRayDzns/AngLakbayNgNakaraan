using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class OutroController : MonoBehaviour
{
     public GameObject[] SlidesTotal;
    public TextMeshProUGUI ReminderText;
    int index = 0;
    int previousindex = 0;
    private float timer = 0f;

    void Start()
    {
        if (ReminderText != null) ReminderText.text = "";

        if (SlidesTotal == null || SlidesTotal.Length == 0)
        {
            Debug.LogWarning("OutroController: No slides assigned in SlidesTotal.");
            return;
        }
        
        for (int i = 0; i < SlidesTotal.Length; i++)
            SlidesTotal[i].SetActive(false);

        SlidesTotal[index].SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                timer = 0f;
                if (ReminderText != null) ReminderText.text = "";
                nextSlide();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            timer = 0f;
            if (ReminderText != null) ReminderText.text = "";
            nextSlide();
        }

        Reminder();
    }

    public void Reminder()
    {
        if (ReminderText == null) return;

        if (timer >= 15f)
        {
            ReminderText.text = "Click to Continue...";
            Debug.Log("Reminder shown");
        }
    }

    public void nextSlide()
    {
        previousindex = index;
        index++;

        if (SlidesTotal == null || index >= SlidesTotal.Length)
        {
            SceneManager.LoadScene("TitleScreen");
            return;
        }

        SlidesTotal[previousindex].SetActive(false);
        SlidesTotal[index].SetActive(true);
    }
}
