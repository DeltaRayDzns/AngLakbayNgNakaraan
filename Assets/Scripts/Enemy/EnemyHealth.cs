using UnityEngine;
using System.Collections; 

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Damage UI")]
    public float hitFlashDuration = 0.5f;                      
    public Color hitFlashColor = new Color(1f, 0f, 0f, 1f);  
    private SpriteRenderer[] flashSprites;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    void Start()
    {
        currentHealth = maxHealth;

        flashSprites = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[flashSprites.Length];
        for (int i = 0; i < flashSprites.Length; i++)
            originalColors[i] = flashSprites[i].color;
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Damage Taken " + currentHealth);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDamageSFX();

        currentHealth -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage! Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            StartCoroutine(FlashThenDie());
            return;
        }

        TriggerFlash();
    }

    void TriggerFlash()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashSprites.Length; i++)
            if (flashSprites[i]) flashSprites[i].color = hitFlashColor;

        yield return new WaitForSecondsRealtime(hitFlashDuration);

        for (int i = 0; i < flashSprites.Length; i++)
            if (flashSprites[i]) flashSprites[i].color = originalColors[i];

        flashRoutine = null;
    }

    IEnumerator FlashThenDie()
    {
        for (int i = 0; i < flashSprites.Length; i++)
            if (flashSprites[i]) flashSprites[i].color = hitFlashColor;

        yield return new WaitForSecondsRealtime(hitFlashDuration);

        for (int i = 0; i < flashSprites.Length; i++)
            if (flashSprites[i]) flashSprites[i].color = originalColors[i];

        Die();
    }

    void Die()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDeathSFX();

        Destroy(gameObject);
    }
}
