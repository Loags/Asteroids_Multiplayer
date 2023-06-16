using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkWavesController : NetworkBehaviour
{
    public static NetworkWavesController Singleton { get; private set; }

    [Header("Choose value between 1 and 100")] [SerializeField]
    private float increasePercentage;

    [SerializeField] private int amountToSpawn;
    [SerializeField] private int timeBeforeNextRound;
    private NetworkVariable<int> amountAlive = new NetworkVariable<int>();
    private Coroutine waveClearCoroutine;
    private bool waveCleared = false;

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        amountAlive.OnValueChanged += AmountChanged;
        NetworkObjectPool.InstatiatePoolDone += StartWaveServerRpc;
    }

    public void ObjectStatusTracker(ObjectTyp _typ)
    {
        switch (_typ)
        {
            case ObjectTyp.Obstacle:
                amountAlive.Value -= 1;
                break;
            case ObjectTyp.Projectile:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_typ), _typ, null);
        }
    }

    private void AmountChanged(int previousvalue, int newvalue)
    {
        if (newvalue > 0) return;
        amountAlive.Value = 0;

        if (waveCleared) return;
        WaveCleared();
    }

    [ServerRpc]
    private void StartWaveServerRpc()
    {
        NetworkObstacleController.Singleton.SpawnObstacles(amountToSpawn);
        amountAlive.Value = amountToSpawn;
        waveCleared = false;
    }


    private void WaveCleared()
    {
        waveCleared = true;
        IncreaseAmountToSpawn();

        if (waveClearCoroutine != null)
            StopCoroutine(waveClearCoroutine);

        waveClearCoroutine = StartCoroutine(WaitBeforeNextRound(timeBeforeNextRound));
    }

    private IEnumerator WaitBeforeNextRound(float _waitDuration)
    {
        yield return new WaitForSeconds(_waitDuration);
        StartWaveServerRpc();
    }

    private void IncreaseAmountToSpawn() =>
        amountToSpawn = Mathf.CeilToInt(amountToSpawn * (1f + increasePercentage / 100));
}