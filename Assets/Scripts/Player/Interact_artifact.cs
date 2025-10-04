using UnityEngine;
using UnityEngine.InputSystem;

public class Interact_artifact : MonoBehaviour
{
    public GameObject[] artefacts;
    public GameObject[] artefact_Panels;
    public GameObject[] Interact_UI;
    public GameObject X_Button;

    void Start()
    {
        gameObject.SetActive(true);

        for (int x = 0; x < artefacts.Length; x++)
        {
            Interact_UI[x].SetActive(false);
            artefact_Panels[x].SetActive(false);
        }


    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        for (int i = 0; i < artefacts.Length; i++)
        {
            if (other.gameObject == artefacts[i])
            {
                Interact_UI[i].SetActive(true);
                Debug.Log(artefacts[i].name);
            }

        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        for (int i = 0; i < artefacts.Length; i++)
        {
            if (other.gameObject == artefacts[i])
            {
                Interact_UI[i].SetActive(true);

                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    Debug.Log("Pressed F and Paused: " + artefacts[i].name);
                    artefact_Panels[i].SetActive(true);
                    X_Button.SetActive(true);
                    Time.timeScale = 0f; // pause
                }
            }
        }
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        for (int i = 0; i < artefacts.Length; i++)
        {
            if (other.gameObject == artefacts[i])
            {
                Interact_UI[i].SetActive(false);
                Debug.Log("Exit");

            }

        }
    }

    void Update()
    {
        if (!X_Button.activeSelf)
        {
            Debug.Log("Continue");
            Time.timeScale = 1f;
        }
    }

}
