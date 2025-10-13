using UnityEngine;
using Pathfinding;

public class Enemy_POV : MonoBehaviour
{
    private EnemyAI_controller enemyAIController;

    void Start()
    {
        enemyAIController = GetComponentInParent<EnemyAI_controller>();
        enemyAIController.target = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            enemyAIController.target = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            enemyAIController.target = null;
    }
}