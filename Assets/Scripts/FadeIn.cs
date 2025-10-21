using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    [Header("Fade UI")]
    public Image fadePanel; 
    public float fadeDuration = 0.5f;
    public float holdTime = 0.2f;

    [Header("Victory")]
    public GameObject victoryPanel;
    public bool revealPanelAfterWhite = true;
    public float revealDelay = 0.05f; 

    void Awake()
    {
        if (!fadePanel) fadePanel = GetComponent<Image>();

        if (fadePanel)
        {
            var c = fadePanel.color;
            fadePanel.color = new Color(1f, 1f, 1f, 0f);
            fadePanel.raycastTarget = true;
        }

        if (victoryPanel) victoryPanel.SetActive(false);
    }

    public IEnumerator DoVictoryFade(System.Action onVictoryShown = null)
    {
        yield return Fade(0f, 1f);

        if (holdTime > 0f)
            yield return new WaitForSecondsRealtime(holdTime);

        Time.timeScale = 0f;

        if (victoryPanel) victoryPanel.SetActive(true);

        onVictoryShown?.Invoke();

        if (revealPanelAfterWhite)
        {
            if (revealDelay > 0f)
                yield return new WaitForSecondsRealtime(revealDelay);

            yield return Fade(1f, 0f);
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        if (!fadePanel) yield break;

        float t = 0f;
        var c = fadePanel.color;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t / fadeDuration));
            c.a = a;
            fadePanel.color = c;
            yield return null;
        }
        
        c.a = to;
        fadePanel.color = c;
    }
}
