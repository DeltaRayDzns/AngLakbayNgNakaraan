using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    [Header("Fade UI")]
    public Image fadePanel;                  // White fullscreen Image
    public float fadeDuration = 0.5f;        // Unscaled seconds
    public float holdTime = 0.2f;            // Unscaled seconds to stay white before victory

    [Header("Victory")]
    public GameObject victoryPanel;          // Victory UI to show (make sure it renders above the overlay)

    [Header("UI To Hide During Sequence")]
    public GameObject interactPrompt;        // e.g. “Press E”
    public GameObject[] uiToHide;            // e.g. Pause, Weapon_System, HUD, etc.

    void Awake()
    {
        if (!fadePanel) fadePanel = GetComponent<Image>();
        if (fadePanel)
        {
            // Start transparent white and block clicks by default
            var c = fadePanel.color;
            fadePanel.color = new Color(1f, 1f, 1f, 0f);
            fadePanel.raycastTarget = true;
        }

        if (victoryPanel) victoryPanel.SetActive(false);
    }

    /// <summary>
    /// Full victory flow owned here:
    /// 1) Hide listed UI (except this overlay)
    /// 2) Fade to white (unscaled)
    /// 3) Hold, then pause game
    /// 4) Show victory panel
    /// NOTE: No fade-back — overlay stays after fade.
    /// </summary>
    public IEnumerator PlayVictorySequence()
    {
        // Hide requested UI (never hide the overlay object itself)
        if (interactPrompt) interactPrompt.SetActive(false);
        if (uiToHide != null)
        {
            for (int i = 0; i < uiToHide.Length; i++)
            {
                var go = uiToHide[i];
                if (!go) continue;
                if (go == gameObject) continue;
                go.SetActive(false);
            }
        }

        // Ensure overlay is active & enabled
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!enabled) enabled = true;

        // 2) Fade to white
        yield return StartCoroutine(Fade(0f, 1f));

        // 3) Hold at full white (still unscaled), then pause game
        if (holdTime > 0f)
            yield return new WaitForSecondsRealtime(holdTime);

        Time.timeScale = 0f;

        // 4) Show victory UI and ensure it draws above the overlay
        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
            victoryPanel.transform.SetAsLastSibling(); // draw on top if same canvas
        }

        // IMPORTANT: We DO NOT fade back. The overlay stays as-is (white).
        // If you want clicks on the victory panel, ensure it renders above this image.
        // (Optionally, you can turn off raycast on the overlay if needed:)
        // if (fadePanel) fadePanel.raycastTarget = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (!fadePanel) yield break;

        float t = 0f;
        var c = fadePanel.color;

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
