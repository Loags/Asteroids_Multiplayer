using UnityEngine;

public class DontDestroyOnLoadHelper : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddToDontDestroyOnLoad();
    }
}