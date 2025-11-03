using UnityEngine;

public class Tutorial_sequence : MonoBehaviour
{
	public GameObject Weapon_System; 
	public GameObject ControlButtons; 
 	public GameObject Pause; 
	public GameObject ChargeJump; 
	public GameObject HealthUI;
	public GameObject ShieldUI;

    void Start()
    {
        Time.timeScale = 0f;
		Weapon_System.SetActive(false);
		ControlButtons.SetActive(false);
		Pause.SetActive(false);
		ChargeJump.SetActive(false);
		HealthUI.SetActive(false);
		ShieldUI.SetActive(false); 

		gameObject.SetActive(true);
    }

    // Update is called once per frame
	public void FinishTutorial() 
	{
		Time.timeScale = 1f; 

		Weapon_System.SetActive(true);
		ControlButtons.SetActive(true);
		Pause.SetActive(true);
		ChargeJump.SetActive(true);
		HealthUI.SetActive(true);
		ShieldUI.SetActive(true); 

		gameObject.SetActive(false);
	} 

	public void SkipTutorial() 
	{
		Time.timeScale = 1f;
		
		Weapon_System.SetActive(true);
		ControlButtons.SetActive(true);
		Pause.SetActive(true);
		ChargeJump.SetActive(true);
		HealthUI.SetActive(true);
		ShieldUI.SetActive(true); 

		gameObject.SetActive(false);
	}
}
