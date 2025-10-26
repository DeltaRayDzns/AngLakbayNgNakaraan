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
	
    void Start()
    {
        Interact_UI.SetActive(false);
		Dialogue_Panel.SetActive(false);
    }
    
	void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.CompareTag("Player")) 
		{
			Interact_UI.SetActive(true);
			Debug.Log("Player can interact");
		}
	}

    void OnTriggerStay2D(Collider2D other) 
	{
		if (other.CompareTag("Player")) 
		{
			Interact_UI.SetActive(true);
			if (Keyboard.current.fKey.wasPressedThisFrame) 
			{
				Debug.Log("Pressed F and Paused: Talk with " + gameObject.name);
				
				Weapon_System.SetActive(false);
				Pause.SetActive(false);
				
				Time.timeScale = 0f;
				Interact_UI.SetActive(true);
				Dialogue_Panel.SetActive(true); 
			}
		}
	}

	void OnTriggerExit2D (Collider2D other) 
	{
		if (other.CompareTag("Player")) 
		{
			Interact_UI.SetActive(true);
			Debug.Log("Player can interact");
		}
	}

	public void DialogueContinue() 	
	{
		Time.timeScale = 1f;

        Weapon_System.SetActive(true);
		Pause.SetActive(true);

		Debug.Log("Continue Game: Bye " + gameObject.name);
		Dialogue_Panel.SetActive(false); 
	}

}
