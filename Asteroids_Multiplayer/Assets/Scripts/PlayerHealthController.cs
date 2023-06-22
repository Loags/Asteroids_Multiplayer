using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealthController : NetworkBehaviour
{
    [SerializeField] private int playerMaxHealth;
    [SerializeField] private int playerCurrentHealth;
    [SerializeField] private float invincibilityTime;
    [SerializeField] private float deathTime;
    [SerializeField] private bool invincible;
    private ObjectBlink blinkEffect;
    private PlayerController playerController;

    public delegate void HealthChanged();

    public event HealthChanged OnHealthChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        blinkEffect = GetComponent<ObjectBlink>();
        playerController = GetComponent<PlayerController>();
    }

    public int GetCurrentHealth()
    {
        return playerCurrentHealth;
    }

    public int GetMaxHealth()
    {
        return playerMaxHealth;
    }

    public void TakeDamage()
    {
        if (invincible)
        {
            return;
        }

        playerCurrentHealth--;
        OnHealthChanged?.Invoke();

        if (playerCurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine(invincibilityTime));
            StartBlinkEffectServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private IEnumerator InvincibilityCoroutine(float _time, bool _death = false)
    {
        invincible = true;

        yield return new WaitForSeconds(_time);

        invincible = false;
        StopBlinkEffectServerRpc(GetComponent<NetworkObject>().NetworkObjectId);

        if (!_death) yield break;

        playerController.blockInput = false;
        Respawn();
    }

    private void Respawn()
    {
        playerMaxHealth = 3;
        playerCurrentHealth = 3;
        OnHealthChanged?.Invoke();
    }

    private void Die()
    {
        playerController.blockInput = true;
        StartCoroutine(InvincibilityCoroutine(deathTime, true));
        StartBlinkEffectServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartBlinkEffectServerRpc(ulong _playerObjectID)
    {
        StartBlinkEffectClientRpc(_playerObjectID);
    }

    [ClientRpc]
    private void StartBlinkEffectClientRpc(ulong _playerObjectID)
    {
        List<GameObject> playerGameObjects = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var playerGameObject in playerGameObjects)
        {
            if (playerGameObject.GetComponent<NetworkObject>().NetworkObjectId != _playerObjectID) continue;

            if (blinkEffect != null)
                blinkEffect.StartBlinking();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopBlinkEffectServerRpc(ulong _playerObjectID)
    {
        StopBlinkEffectClientRpc(_playerObjectID);
    }

    [ClientRpc]
    private void StopBlinkEffectClientRpc(ulong _playerObjectID)
    {
        List<GameObject> playerGameObjects = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var playerGameObject in playerGameObjects)
        {
            if (playerGameObject.GetComponent<NetworkObject>().NetworkObjectId != _playerObjectID) continue;

            if (blinkEffect != null)
                blinkEffect.StopBlinking();
        }
    }

    public void IncreaseMaxHealth()
    {
        playerMaxHealth += 1;
        OnHealthChanged?.Invoke();
    }

    public bool IncreaseCurrentHealth()
    {
        if (playerCurrentHealth >= playerMaxHealth)
            return false;

        playerCurrentHealth += 1;
        OnHealthChanged?.Invoke();
        return true;
    }
}