using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class NPC_Talk : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject Dialogue_Panel;
    public GameObject Interact_UI;

    [Header("Player_UI")]
    public GameObject Weapon_System;
    public GameObject Pause;
	public GameObject ControlButtons; 
	

    private bool playerInRange = false;

    void Start()
    {
        Interact_UI.SetActive(false);
        Dialogue_Panel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Interact_UI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Interact_UI.SetActive(false);
        }
    }

    public void InteractButtonPressed()
    {
        if (!playerInRange)
        {
            Debug.Log("NPC Interacts: NULL");
            return;
        }


        if (Weapon_System != null) Weapon_System.SetActive(false);
        if (Pause != null) Pause.SetActive(false);

        Time.timeScale = 0f;

        if (Interact_UI != null) Interact_UI.SetActive(true);
		if (ControlButtons != null) ControlButtons.SetActive(false);
        if (Dialogue_Panel != null) Dialogue_Panel.SetActive(true);
    }

    public void DialogueContinue()
    {
        Time.timeScale = 1f;

        if (Weapon_System != null) Weapon_System.SetActive(true);
        if (Pause != null) Pause.SetActive(true);

		
		if (ControlButtons != null) ControlButtons.SetActive(true);

        Debug.Log("Continue Game: Bye " + gameObject.name);
        if (Dialogue_Panel != null) Dialogue_Panel.SetActive(false);
    }
}
