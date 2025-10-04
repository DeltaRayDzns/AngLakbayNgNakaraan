using UnityEngine;
using UnityEngine.UI;

public class Shield_Consume : MonoBehaviour
{

    public Player_health playerHealth;
    
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerHealth.IsShieldActive())
            {
                playerHealth.ActivateShield();
                gameObject.SetActive(false);
                Debug.Log("Shield activated");
            }
            else
            {
                Debug.Log("Shield already activated");
            }
        }
   
    }
}
