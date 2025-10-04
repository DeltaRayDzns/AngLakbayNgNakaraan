using UnityEngine;

public class Health_Consume : MonoBehaviour
{
    public Player_health playerHealth;
    void Start()
    {
        gameObject.SetActive(true);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.Heal(1);
                gameObject.SetActive(false);
                Debug.Log("Health Consumed");
            }
            else
            {
                Debug.Log("Full health");
            }
        }
    }
}
