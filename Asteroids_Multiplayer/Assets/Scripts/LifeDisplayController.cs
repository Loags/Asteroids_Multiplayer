using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplayController : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform targetAnchor;

    private List<Image> currentSpawnedImages = new();
    private PlayerHealthController playerHealthController;
    private PlayerController playerController;

    private Color normal = Color.white;
    private Color highlight = Color.red;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        playerController = GetterPlayerController.GetPlayerControllerWithID(NetworkManager.Singleton.LocalClientId);
        playerHealthController = playerController.playerHealthController;

        for (int i = 0; i < playerHealthController.GetCurrentHealth(); i++)
            InitializePrefab();

        playerHealthController.OnHealthChanged += UpdateLifeVisuals;
    }

    private void OnDestroy()
    {
        playerHealthController.OnHealthChanged -= UpdateLifeVisuals;
    }

    private void InitializePrefab()
    {
        currentSpawnedImages.Add(Instantiate(prefabToSpawn, targetAnchor).GetComponent<Image>());
    }

    private void UpdateLifeVisuals()
    {
        // Check Max Health
        if (currentSpawnedImages.Count < playerHealthController.GetMaxHealth())
        {
            int difference = playerHealthController.GetMaxHealth() - currentSpawnedImages.Count;
            for (int j = 0; j < difference; j++)
                InitializePrefab();
        }
        else
        {
            DestroyAllSpawnedImages();
            for (int i = 0; i < playerHealthController.GetMaxHealth(); i++)
                InitializePrefab();
        }

        // Check Current Health
        foreach (var image in currentSpawnedImages)
            image.color = normal;

        for (int i = 0; i < playerHealthController.GetCurrentHealth(); i++)
            currentSpawnedImages[i].color = highlight;
    }

    private void DestroyAllSpawnedImages()
    {
        foreach (var image in currentSpawnedImages)
        {
            Destroy(image.gameObject);
        }

        currentSpawnedImages.Clear();
    }
}