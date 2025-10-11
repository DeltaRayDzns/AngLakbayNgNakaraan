using UnityEngine;
using System.Collections;

public class Background_Manager : MonoBehaviour
{
    [Header("Background Settings")]
    [SerializeField] private CanvasGroup[] backgrounds; 
    [SerializeField] private float fadeDuration = 2f;  
    [SerializeField] private float displayDuration = 5f; 

    private int currentIndex = 0;

    void Start()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].alpha = (i == 0) ? 1 : 0;
        }

        StartCoroutine(FadeLoop());
    }

    private IEnumerator FadeLoop()
    {
        while (true)
        {
            int nextIndex = (currentIndex + 1) % backgrounds.Length;

            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(FadeToNextBackground(currentIndex, nextIndex));

            currentIndex = nextIndex;
        }
    }

    private IEnumerator FadeToNextBackground(int fromIndex, int toIndex)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            backgrounds[fromIndex].alpha = Mathf.Lerp(1f, 0f, t);
            backgrounds[toIndex].alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        backgrounds[fromIndex].alpha = 0f;
        backgrounds[toIndex].alpha = 1f;
    }
}
