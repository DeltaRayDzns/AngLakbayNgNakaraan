using UnityEngine;
using System.Collections;
using TMPro; 
public class Boss_warning : MonoBehaviour
{
    [Header("Boss Barriers")]
    [SerializeField]
    private GameObject PreBoss_barrier;
    
    [SerializeField]
    private GameObject PostBoss_barrier;
    public GameObject Warning_trigger; 
    
    public GameObject Warning_panel;
    public GameObject Pausebtn;
    public GameObject Weapon_System; 
    public GameObject ControlButtons;
    void Start()
    {
        PreBoss_barrier.SetActive(true);
        Warning_trigger.SetActive(true);
        PostBoss_barrier.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            
            Pausebtn.SetActive(false);
            Weapon_System.SetActive(false);
            ControlButtons.SetActive(false);
            Warning_panel.SetActive(true);
        }
    }

    public void Boss_cancel()
    {
        Time.timeScale = 1f;
            
        Pausebtn.SetActive(true);
        Weapon_System.SetActive(true);
        ControlButtons.SetActive(true);
        Warning_panel.SetActive(false);
    }

    public void Boss_confirm()
    {
        Time.timeScale = 1f;
            
        Pausebtn.SetActive(true);
        Weapon_System.SetActive(true);
        ControlButtons.SetActive(true);
        Warning_panel.SetActive(false);
        
        //barriers activate and deactivate
        PreBoss_barrier.SetActive(false);
        Warning_trigger.SetActive(false);
        PostBoss_barrier.SetActive(true);
    }
}
