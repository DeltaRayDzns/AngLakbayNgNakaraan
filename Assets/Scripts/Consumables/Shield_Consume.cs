using UnityEngine;
using UnityEngine.UI;

public class Shield_Consume : MonoBehaviour
{

    public Player_health playerHealth;
    public ShrinkItem shrinkItem; 
    
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
                gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(shrinkItem.Shrinktofalse());
                playerHealth.ActivateShield();
                Debug.Log("Shield activated");
            }
            else
            {
                Debug.Log("Shield already activated");
            }
        }
   
    }
}
