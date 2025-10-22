using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Boss_FirstPage : MonoBehaviour
{
    [Header("Pickup")]
    public string playerTag = "Player";

    [Header("Effects")]
    [SerializeField] private FadeIn fader;         // Scene object with FadeIn
    [SerializeField] private bool shrinkAfterFade = true;

    private bool pickedUp = false;

    void Awake()
    {
        // Auto-find active fader in the scene if not assigned manually
        if (!fader)
        {
#if UNITY_2023_1_OR_NEWER
        fader = FindFirstObjectByType<FadeIn>(FindObjectsInactive.Exclude);
#else
            fader = FindObjectOfType<FadeIn>(false); // Only search active objects
#endif
        }

        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.freezeRotation = true;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!pickedUp && other.CompareTag(playerTag))
            StartCoroutine(CollectRoutine());
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (!pickedUp && c.collider.CompareTag(playerTag))
            StartCoroutine(CollectRoutine());
    }

    private IEnumerator CollectRoutine()
    {
        pickedUp = true;

        // (Optional) keep collider ON if you want it to still collide while shrinking.
        // If you prefer to stop further triggers, uncomment the loop below:
        // foreach (var col in GetComponentsInChildren<Collider2D>()) col.enabled = false;

        // Start fader sequence (handles UI hiding + pause + victory)
        if (fader)
            yield return StartCoroutine(fader.PlayVictorySequence());
        else
            Debug.LogError("[Boss_FirstPage] No FadeIn found in scene. Skipping fade.");

        // Shrink (or hide) the page item
        var shrink = GetComponent<ShrinkItem>();
        if (shrink && shrinkAfterFade)
            yield return StartCoroutine(shrink.Shrinktofalse());
        else
            gameObject.SetActive(false);
    }
}