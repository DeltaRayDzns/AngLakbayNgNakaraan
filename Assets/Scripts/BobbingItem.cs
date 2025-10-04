using UnityEngine;

public class BobbingItem : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.4f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {

        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}