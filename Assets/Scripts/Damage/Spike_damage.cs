using UnityEngine;

public class Spike_damage : MonoBehaviour
{
    public Player_health playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(1);
        }

    }
}
