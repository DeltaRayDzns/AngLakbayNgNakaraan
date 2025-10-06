using UnityEngine;

public class Health_Consume : MonoBehaviour
{
    public Player_health playerHealth;
    public ShrinkItem shrinkItem; 
    
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
                gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(shrinkItem.Shrinktofalse());
                playerHealth.Heal(1);
                Debug.Log("Health Consumed");
            }
            else
            {
                Debug.Log("Full health");
            }
        }
    }
}
