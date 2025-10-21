using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Boss_FirstPage : MonoBehaviour
{
    [Header("UI")]
    public GameObject Interact_UI;
    public GameObject[] PlayerUI;

    [Header("Pickup")]
    public string playerTag = "Player";

    [Header("Effects")]
    [SerializeField] private FadeIn fader;
    [SerializeField] private bool shrinkAfterFade = true;

    private bool pickedUp = false;

    void Awake()
    {
        if (!fader)
        {
        #if UNITY_2023_1_OR_NEWER
            fader = FindFirstObjectByType<FadeIn>(FindObjectsInactive.Include);
        #else
            fader = FindObjectOfType<FadeIn>(true);
        #endif
            if (!fader)
                Debug.LogWarning("[Boss_FirstPage] No FadeIn found in scene. Assign one to 'Fader' in the Inspector.");
        }

        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.freezeRotation = true;

        var rootCol = GetComponent<Collider2D>();
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
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;

        if (Interact_UI) Interact_UI.SetActive(false);

        if (PlayerUI != null)
        {
            for (int i = 0; i < PlayerUI.Length; i++)
            {
                var ui = PlayerUI[i];
                if (!ui) continue;

                if (fader && ui == fader.gameObject) continue;

                ui.SetActive(false);
            }
        }

        if (fader)
        {
            if (!fader.gameObject.activeSelf)
                fader.gameObject.SetActive(true);

            if (!fader.enabled) fader.enabled = true;

            Debug.Log("[Boss_FirstPage] Starting victory fade…");
            yield return StartCoroutine(fader.DoVictoryFade());
        }
        else
        {
            Debug.LogError("[Boss_FirstPage] Fader not set/found. No fade will play.");
        }

        var shrink = GetComponent<ShrinkItem>();
        if (shrink && shrinkAfterFade)
            yield return StartCoroutine(shrink.Shrinktofalse());
        else
            gameObject.SetActive(false);
    }
}
