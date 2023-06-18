using UnityEngine;

public class DontDestroyOnLoadHelper : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}