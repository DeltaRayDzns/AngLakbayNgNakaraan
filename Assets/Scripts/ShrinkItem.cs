using UnityEngine;
using System.Collections;

public class ShrinkItem : MonoBehaviour
{
    public float shrinkVariable;
    public Vector3 finalScale = new Vector3(0f, 0f, 0f);
    
    public IEnumerator Shrinktofalse()
    {
        while (transform.localScale.magnitude > finalScale.magnitude + 0.01f)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                finalScale,
                Time.unscaledDeltaTime * shrinkVariable
            );
            yield return null;
        }
        transform.localScale = finalScale;
        gameObject.SetActive(false);
    }

}
