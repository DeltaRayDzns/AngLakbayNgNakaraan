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

    [Header("UI To Hide During Sequence")]
    public GameObject interactPrompt; 
    public GameObject[] uiToHide; 

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

    public IEnumerator PlayVictorySequence()
    {
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

        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (!enabled) enabled = true;

        yield return StartCoroutine(Fade(0f, 1f));

        if (holdTime > 0f)
            yield return new WaitForSecondsRealtime(holdTime);

        Time.timeScale = 0f;

        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
            victoryPanel.transform.SetAsLastSibling(); 
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
            c.a = Mathf.Lerp(from, to, Mathf.Clamp01(t / fadeDuration));
            fadePanel.color = c;
            yield return null;
        }

        c.a = to;
        fadePanel.color = c;
    }
}
