using UnityEngine;
using UnityEngine.UI; 

public class Player_health : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("HealthIcons")] 
    public GameObject[] heart_Icons;
    public GameObject[] heart_empty_Icons;
    
    [Header("ShieldsIcons")]
    public GameObject shield_Icons;
    public GameObject shield_broken_Icons;
    private bool Shield_active = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (Shield_active == false)
        {
            shield_Icons.SetActive(false);
            shield_broken_Icons.SetActive(true);
        }
    }

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        if (Shield_active)
        {
            Shield_active = false;
            shield_Icons.SetActive(false);
            shield_broken_Icons.SetActive(true);
        }
        else
        {
            currentHealth -= damage;
            if (currentHealth < 0) currentHealth = 0;
        }

        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        for (int i = 0; i < heart_Icons.Length; i++){
            if (i < currentHealth)
            {
                heart_Icons[i].SetActive(true);
                heart_empty_Icons[i].SetActive(false);

            }
            else
            {
                heart_Icons[i].SetActive(false);
                heart_empty_Icons[i].SetActive(true);
            }
        }
    }

    public void ActivateShield()
    {
        Shield_active = true;
        shield_Icons.SetActive(true);
        shield_broken_Icons.SetActive(false);
    }
}
