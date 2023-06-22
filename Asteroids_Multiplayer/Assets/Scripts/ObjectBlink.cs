using System.Collections;
using UnityEngine;

public class ObjectBlink : MonoBehaviour
{
    private float blinkInterval = 0.2f;
    private Renderer objectRenderer;
    private Coroutine blinkCoroutine;

    private void OnEnable()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    public void StartBlinking()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        

        blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        

        objectRenderer.enabled = true;
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            objectRenderer.enabled = !objectRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}