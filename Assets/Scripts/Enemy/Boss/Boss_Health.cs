using UnityEngine;

public class Boss_Health : MonoBehaviour
{
    [Header("Boss Health Nodes")]
    public GameObject[] BossHealth_Nodes;

    [SerializeField]
    public int Boss_maxHealth { get; private set; }
    [SerializeField]
    public int Boss_currentHealth { get; private set; }

    private bool bossDefeated = false;

    [SerializeField] 
    private GameObject FirstPage;

    void Start()
    {
        if (BossHealth_Nodes == null || BossHealth_Nodes.Length == 0)
            BossHealth_Nodes = GameObject.FindGameObjectsWithTag("BossNode");
        
        Boss_maxHealth = BossHealth_Nodes != null ? BossHealth_Nodes.Length : 0;
        Boss_currentHealth = Boss_maxHealth;

        Debug.Log($"[Boss_Health] Boss initialized with {Boss_currentHealth}/{Boss_maxHealth} HP.");
    }

    void Update()
    {
        if (!bossDefeated && AreAllNodesDestroyed())
        {
            Debug.Log("[Boss_Health] All nodes destroyed → triggering OnBossDefeated()");
            OnBossDefeated();
        }
    }
    public void Damage(int amount)
    {
        if (amount <= 0 || bossDefeated) return;

        Boss_currentHealth -= amount;
        if (Boss_currentHealth < 0) Boss_currentHealth = 0;

        Debug.Log($"[Boss_Health] Boss took {amount} damage! Current HP: {Boss_currentHealth}/{Boss_maxHealth}");

        if (Boss_currentHealth <= 0)
        {
            OnBossDefeated();
        }
    }

    private bool AreAllNodesDestroyed()
    {
        if (BossHealth_Nodes == null || BossHealth_Nodes.Length == 0)
            return true;

        foreach (var node in BossHealth_Nodes)
        {
            if (node != null && node.activeSelf)
                return false;
        }

        return true;
    }

    private void OnBossDefeated()
    {
        if (bossDefeated) return;
        bossDefeated = true;
        
        
        Vector3 pos = gameObject.transform.position;
        if (FirstPage != null)
        {
            Instantiate(FirstPage, pos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[Boss_Health] First page is null");
        }


        Destroy(gameObject);
        Debug.Log("[Boss_Health] Boss defeated!");
    }
    
    
}
