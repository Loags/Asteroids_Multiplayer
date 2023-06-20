using System.Collections.Generic;
using UnityEngine;

public static class DontDestroyOnLoadController
{
    private static List<GameObject> objectsInDontDestroy = new();


    public static void AddToDontDestroyOnLoad(this GameObject gameObject)
    {
        Object.DontDestroyOnLoad(gameObject);
        objectsInDontDestroy.Add(gameObject);
    }

    public static void DestroyAll()
    {
        Debug.Log("Destroy everything in dontdestory: " + objectsInDontDestroy.Count);
        foreach (var gameObject in objectsInDontDestroy)
        {
            if (gameObject != null)
                Object.Destroy(gameObject);
        }

        objectsInDontDestroy.Clear();
    }
}