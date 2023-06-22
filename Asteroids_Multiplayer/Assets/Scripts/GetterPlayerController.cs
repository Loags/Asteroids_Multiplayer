using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using Unity.Netcode;
using UnityEngine;

public static class GetterPlayerController
{
    public static PlayerController GetPlayerControllerWithID(ulong _playerID)
    {
        PlayerController playerController = null;
        List<PlayerController> tempControllers = Object.FindObjectsOfType<PlayerController>().ToList();
        foreach (var tempPlayerController in tempControllers)
        {
            if (tempPlayerController.playerID != _playerID) continue;
            playerController = tempPlayerController;
        }

        return playerController;
    }
}