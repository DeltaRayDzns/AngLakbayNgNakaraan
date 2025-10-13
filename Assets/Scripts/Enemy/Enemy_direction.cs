using UnityEngine;

public class Enemy_direction : MonoBehaviour
{
    [SerializeField] private EnemyAI_controller enemyAI; 
    [SerializeField] private Transform enemyGFX;          

    void Start()
    {
        if (enemyAI == null)
            enemyAI = GetComponentInParent<EnemyAI_controller>();

        if (enemyGFX == null)
            enemyGFX = transform; 
    }

    void Update()
    {
        if (enemyAI == null || enemyAI.target == null) return;

        
        float directionX = enemyAI.target.position.x - transform.position.x;

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        if (directionX > 0.05f)
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        else if (directionX < -0.05f)
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
    }
}