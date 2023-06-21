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
    [SerializeField] private bool invincible;
    private ObjectBlink blinkEffect;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        blinkEffect = GetComponent<ObjectBlink>();
    }

    public void TakeDamage()
    {
        Debug.Log("Player TakeDamage");
        if (invincible)
        {
            return;
        }

        playerCurrentHealth--;

        if (playerCurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
            StartBlinkEffectClientRpc(GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        invincible = true;

        yield return new WaitForSeconds(invincibilityTime);

        invincible = false;
        StopBlinkEffect();
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        StopBlinkEffect();
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

    private void StopBlinkEffect()
    {
        if (blinkEffect != null)
        {
            blinkEffect.StopBlinking();
        }
    }

    public void IncreaseMaxHealth()
    {
        playerMaxHealth += 1;
    }

    public bool IncreaseCurrentHealth()
    {
        if (playerCurrentHealth >= playerMaxHealth)
            return false;

        playerCurrentHealth += 1;
        return true;
    }
}