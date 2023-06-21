using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class PSDestroy : NetworkBehaviour
{
    // Use this for initialization
    void Start()
    {
        //if (IsHost)
        //StartDespawnAfterDelay();
        Destroy(gameObject, 1f);
    }

    public void StartDespawnAfterDelay()
    {
        StartCoroutine(DespawnAfterDelay());
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        GetComponent<NetworkObject>().Despawn();
    }
}