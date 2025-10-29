using UnityEngine;
using UnityEngine.InputSystem;

public class Interact_artifact : MonoBehaviour
{
    public GameObject[] artefacts;
    public GameObject[] artefact_Panels;
    public GameObject[] Interact_UI;
	public GameObject ControlButtons; 
    public GameObject X_Button;

    private int currentIndex = -1;

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
                currentIndex = i;
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
                if (currentIndex == i)
                    currentIndex = -1; 
            }
        }
    }

    public void InteractButtonPressed()
    {
        if (currentIndex == -1)
        {
            Debug.Log("Artefacts interact: null");
            return;
        }

        artefact_Panels[currentIndex].SetActive(true);
        X_Button.SetActive(true);
		ControlButtons.SetActive(false);

        Time.timeScale = 0f;
    }


}
