using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Boss_FirstPage : MonoBehaviour
{
    [Header("UI (optional)")]
    public GameObject Interact_UI;          // e.g. “Press E” prompt
    public GameObject[] PlayerUI;           // UI to hide during the victory sequence (Pause, Weapon_System, etc.)

    [Header("Pickup")]
    public string playerTag = "Player";

    [Header("Effects")]
    [SerializeField] private FadeIn fader;  // Scene object with FadeIn
    [SerializeField] private bool shrinkAfterFade = true;

    private bool pickedUp = false;

    void Awake()
    {
        // Auto-find fader if not wired
        if (!fader)
        {
#if UNITY_2023_1_OR_NEWER
            fader = FindFirstObjectByType<FadeIn>(FindObjectsInactive.Include);
#else
            fader = FindObjectOfType<FadeIn>(true);
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

        // Stop further contacts
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;

        // Let the fader run the whole victory sequence (fade, hold, pause, show panel)
        if (fader)
        {
            // Hide the “interact” prompt before fading
            if (Interact_UI) Interact_UI.SetActive(false);

            yield return StartCoroutine(fader.PlayVictorySequence(Interact_UI, PlayerUI));
        }
        else
        {
            Debug.LogError("[Boss_FirstPage] No FadeIn found in scene. Skipping fade.");
        }

        // Finally, shrink (or just hide) the page item
        var shrink = GetComponent<ShrinkItem>();
        if (shrink && shrinkAfterFade)
            yield return StartCoroutine(shrink.Shrinktofalse());
        else
            gameObject.SetActive(false);
    }
}
