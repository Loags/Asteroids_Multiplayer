using System;
using Netcode.Transports.PhotonRealtime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager instance;

    [SerializeField] private TMP_Text joinCode;
    [SerializeField] private InputField inputField;
    private string connectionCode;
    private PhotonRealtimeTransport photon;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        photon = GetComponent<PhotonRealtimeTransport>();
    }

    public void StartHost()
    {
        string s = Guid.NewGuid().ToString();
        s = s.Substring(0, 5);
        s = s.ToLower();

        photon.RoomName = s;
        joinCode.text = s;
        print(s);

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void JoinGame()
    {
        connectionCode.ToLower();
        if (connectionCode.Length == 5)
        {
            photon.RoomName = connectionCode;
        }

        NetworkManager.Singleton.StartClient();
    }

    public void UpdateConnectionCode(string _code)
    {
        connectionCode = _code;
    }
}