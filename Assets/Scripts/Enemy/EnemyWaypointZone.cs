using UnityEngine;

public class EnemyWaypointZone : MonoBehaviour
{
    public Transform[] localWaypoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI_controller ai = other.GetComponent<EnemyAI_controller>();
            if (ai != null)
            {
                ai.enabled = false; // temporarily disable A* AI
                EnemyWaypointFollower follower = other.GetComponent<EnemyWaypointFollower>();
                if (follower != null)
                {
                    follower.waypoints = localWaypoints;
                    follower.enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI_controller ai = other.GetComponent<EnemyAI_controller>();
            if (ai != null)
            {
                ai.enabled = true; // re-enable A* AI
            }

            EnemyWaypointFollower follower = other.GetComponent<EnemyWaypointFollower>();
            if (follower != null)
            {
                follower.enabled = false;
            }
        }
    }
}