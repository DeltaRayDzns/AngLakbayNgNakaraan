using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    public Image darkPanel;           
    public float fadeDuration = 0.5f; 
    public float holdTime = 0.2f;     
    

    private void Awake()    
    {
        if (darkPanel == null)
            darkPanel = GetComponent<Image>();
        SetAlpha(0f); 
    }

    public IEnumerator DoFade(System.Action onBlack)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        
        onBlack?.Invoke();

        yield return new WaitForSecondsRealtime(holdTime);

        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = darkPanel.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            darkPanel.color = c;
            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (darkPanel != null)
        {
            Color c = darkPanel.color;
            c.a = alpha;
            darkPanel.color = c;
        }
    }
}