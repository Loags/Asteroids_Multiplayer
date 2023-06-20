using System;
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
        {
            // Stop the previous blink coroutine if it's already running
            StopCoroutine(blinkCoroutine);
        }

        blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }

    public void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        // Ensure the object is visible after stopping blinking
        objectRenderer.enabled = true;
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            // Toggle the object's visibility
            objectRenderer.enabled = !objectRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}