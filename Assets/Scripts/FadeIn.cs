using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    [Header("Fade UI")]
    public Image fadePanel;                  // White Image that covers the screen
    public float fadeDuration = 0.5f;        // Time to fade to/from white
    public float holdTime = 0.2f;            // Time to stay fully white before victory

    [Header("Victory")]
    public GameObject victoryPanel;          // The victory UI to show
    public bool revealPanelAfterWhite = true;
    public float revealDelay = 0.05f;        // Small delay before fading white away

    void Awake()
    {
        if (!fadePanel) fadePanel = GetComponent<Image>();
        if (fadePanel)
        {
            // Ensure starts transparent white and blocks clicks
            var c = fadePanel.color;
            fadePanel.color = new Color(1f, 1f, 1f, 0f);
            fadePanel.raycastTarget = true;
        }

        if (victoryPanel) victoryPanel.SetActive(false);
    }

    /// <summary>
    /// Full victory flow:
    /// 1) Hide provided UI (except this fader)
    /// 2) Fade to white (unscaled)
    /// 3) Hold white, then pause game
    /// 4) Show victory panel
    /// 5) Optionally fade the white back out to reveal the panel behind
    /// </summary>
    public IEnumerator PlayVictorySequence(GameObject interactUI, GameObject[] toHide)
    {
        // Hide requested UI (never hide the fader’s own GO)
        if (interactUI) interactUI.SetActive(false);
        if (toHide != null)
        {
            for (int i = 0; i < toHide.Length; i++)
            {
                var go = toHide[i];
                if (!go) continue;
                if (go == gameObject) continue; // don't hide the overlay itself
                go.SetActive(false);
            }
        }

        // Ensure overlay is active & enabled
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!enabled) enabled = true;

        // Fade to white
        yield return StartCoroutine(Fade(0f, 1f));

        // Keep full white briefly (real-time)
        if (holdTime > 0f)
            yield return new WaitForSecondsRealtime(holdTime);

        // Pause gameplay
        Time.timeScale = 0f;

        // Show victory UI
        if (victoryPanel) victoryPanel.SetActive(true);

        // Fade off the white so the victory panel is visible
        if (revealPanelAfterWhite)
        {
            if (revealDelay > 0f)
                yield return new WaitForSecondsRealtime(revealDelay);

            yield return StartCoroutine(Fade(1f, 0f)); // white -> transparent
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        if (!fadePanel) yield break;

        float t = 0f;
        var c = fadePanel.color;

        // Work in real-time so we’re unaffected by timescale
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(t / fadeDuration));
            fadePanel.color = c;
            yield return null;
        }

        c.a = to;
        fadePanel.color = c;
    }
}
