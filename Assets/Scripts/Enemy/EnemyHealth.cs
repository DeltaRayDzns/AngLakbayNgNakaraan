using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 5;
    [SerializeField]
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        
        Debug.Log("Damage Taken" + currentHealth);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDamageSFX();
        
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage! Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDeathSFX();
        
        Destroy(gameObject);
    }
}