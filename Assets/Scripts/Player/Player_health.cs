using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 
using System.Collections; 

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

    [Header("UtilitiesUI")]
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject PauseBtn;

    [Header("Damage UI")]
    public float hitFlashDuration = 0.5f;                      
    public Color hitFlashColor = new Color(1f, 0f, 0f, 1f);  
    private SpriteRenderer[] flashSprites;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    void Start()
    {
        GameOverPanel.SetActive(false);

        currentHealth = maxHealth;
        UpdateHealthUI();

        if (Shield_active == false)
        {
            shield_Icons.SetActive(false);
            shield_broken_Icons.SetActive(true);
        }

        flashSprites = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[flashSprites.Length];
        for (int i = 0; i < flashSprites.Length; i++)
            originalColors[i] = flashSprites[i].color;
    }

    public bool IsShieldActive() => Shield_active;

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDamageSFX();

        TriggerFlash();

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

            if (currentHealth == 0)
            {
                OnDeath();
            }
        }

        UpdateHealthUI();
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

    public void UpdateHealthUI()
    {
        for (int i = 0; i < heart_Icons.Length; i++)
        {
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

    public void OnDeath()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeathSFX();
            AudioManager.Instance.FadeOutMusic(1.5f);
        }

        Time.timeScale = 0f;
        GameOverPanel.SetActive(true);
        PauseBtn.SetActive(false);
    }
    
    public void retry()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        PauseBtn.SetActive(true);
    }
}
