using System;
using UnityEngine;

public class AnchorSetter : MonoBehaviour
{
    [SerializeField] private RectTransform targetRectTransform;

    public void SetAnchorTopStretch()
    {
        // Make sure the target RectTransform is not null
        if (targetRectTransform != null)
        {
            // Set the anchor properties to stretch from the top
            targetRectTransform.anchorMin = new Vector2(0, 1);
            targetRectTransform.anchorMax = new Vector2(1, 1);
            targetRectTransform.pivot = new Vector2(0.5f, 1);
            targetRectTransform.anchoredPosition.Set(0, 0);
        }
        else
        {
            Debug.LogError("Target RectTransform is null. Please assign a valid RectTransform.");
        }
    }
}