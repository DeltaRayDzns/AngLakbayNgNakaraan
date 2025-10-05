using UnityEngine;

public class FallBack_damage : MonoBehaviour
{
    public Player_health playerHealth; 
	public GameObject Player;
    public Transform fallBackPoint;    
    public FadeOut fadeOut;            

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
			Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

			rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Debug.Log("Player entered pit: " + gameObject.name);
			
            if (fadeOut != null)
            {
                StartCoroutine(fadeOut.DoFade(() =>
                {	
					rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    other.transform.position = fallBackPoint.position;

                    if (playerHealth != null)
                        playerHealth.TakeDamage(1);

                    Debug.Log("Player teleported and damaged at pit: " + gameObject.name);
					
                }));
            }
            else
            {
                other.transform.position = fallBackPoint.position;
                if (playerHealth != null)
                    playerHealth.TakeDamage(1);
            }
        }
    }
}
