using UnityEngine;

public class Paused : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
	public void timePaused() 
	{
        Time.timeScale = 0f;
	} 
	public void timeContinue () 
	{
        Time.timeScale = 1f;
	}
}
