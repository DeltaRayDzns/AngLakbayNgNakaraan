using UnityEngine;
using UnityEngine.Tilemaps;

public class Hidden_wall : MonoBehaviour
{
    [SerializeField] Tilemap renderTilemap;
    [SerializeField] float fadeTime = 0.4f;
    [SerializeField, Range(0f,1f)] float hiddenA = 0.12f; 

    Coroutine fadeCo;

    void Reset()
    {
        if (renderTilemap == null) renderTilemap = GetComponentInChildren<Tilemap>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartFade(hiddenA);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartFade(1f);
    }

    void StartFade(float targetA)
    {
        if (renderTilemap == null) return;
        if (fadeCo != null) StopCoroutine(fadeCo);
        fadeCo = StartCoroutine(FadeTo(targetA));
    }

    System.Collections.IEnumerator FadeTo(float targetA)
    {
        Color c = renderTilemap.color;
        float startA = c.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeTime;
            c.a = Mathf.Lerp(startA, targetA, t);
            renderTilemap.color = c;
            yield return null;
        }
        c.a = targetA;
        renderTilemap.color = c;
        fadeCo = null;
    }
}