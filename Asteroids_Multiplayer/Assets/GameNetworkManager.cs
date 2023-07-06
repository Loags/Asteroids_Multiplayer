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
    [SerializeField] private TMP_InputField inputField;
    private string connectionCode;
    private PhotonRealtimeTransport photon;
    private bool lobbyRunningLogged;

    private bool sceneCheckPerformed; // Flag to track if the scene check has been performed
    private bool waitForLobbyCheck = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        photon = GetComponent<PhotonRealtimeTransport>();
        gameObject.AddToDontDestroyOnLoad();
    }

    private void OnEnable()
    {
        Application.quitting += Disconnect;

        if (NetworkManager.Singleton && NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.OnServerStopped += Disconnect;
        else if (NetworkManager.Singleton && NetworkManager.Singleton.IsClient)
            NetworkManager.Singleton.OnClientDisconnectCallback += Disconnect;
    }

    private void OnDisable()
    {
        Application.quitting -= Disconnect;

        if (NetworkManager.Singleton && NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.OnServerStopped -= Disconnect;
        else if (NetworkManager.Singleton && NetworkManager.Singleton.IsClient)
            NetworkManager.Singleton.OnClientDisconnectCallback -= Disconnect;
    }


    private void Update()
    {
        if (NetworkManager.Singleton.IsHost) return;

        if (!sceneCheckPerformed &&
            (SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Game"))
        {
            sceneCheckPerformed = true;
            Invoke(nameof(ToggleWaitForLobbyCheck),
                1f); // Call ToggleWaitForLobbyCheck once after entering Lobby or Game scene
        }

        if (waitForLobbyCheck)
            return;

        // Stop if NetworkManager is null
        if (NetworkManager.Singleton == null)
            return;

        // Stop if the client is still connected to a server
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
            return;

        // Make the Disconnect only be called once
        if (lobbyRunningLogged)
            return;

        Debug.Log("The lobby is no longer running");
        // Mark the message as logged
        lobbyRunningLogged = true;
        Disconnect();
    }

    private void ToggleWaitForLobbyCheck()
    {
        waitForLobbyCheck = false;
    }

    public void StartHost()
    {
        string s = Guid.NewGuid().ToString();
        s = s.Substring(0, 5);
        s = s.ToLower();

        photon.RoomName = s;
        joinCode.text = "Room Code\n" + s;
        joinCode.transform.parent.gameObject.SetActive(true);

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void JoinGame()
    {
        connectionCode?.ToLower();

        if (connectionCode == null || connectionCode.Length != 5)
        {
            WrongCode();
            return;
        }

        photon.RoomName = connectionCode;
        NetworkManager.Singleton.StartClient();
    }

    public void UpdateConnectionCode(string _code) => connectionCode = _code;


    private void WrongCode() => inputField.GetComponent<Image>().color = Color.red;

    public void ResetInputField() => inputField.GetComponent<Image>().color = new Color(100, 100, 100);


    public void Disconnect(bool _value)
    {
        Debug.Log("Disconnect!");

        if (_value && NetworkManager.Singleton && NetworkManager.Singleton.IsHost)
        {
            // Stop the server and return to the main menu
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        DontDestroyOnLoadController.DestroyAll();
    }

    public void Disconnect(ulong _clientID)
    {
        Debug.Log($"Disconnect client: {_clientID}");

        if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
        {
            // Disconnect the specified client and return to the main menu
            NetworkManager.Singleton.DisconnectClient(_clientID);
        }
    }

    private void Disconnect()
    {
        Debug.Log("Disconnect!");

        if (NetworkManager.Singleton.IsHost)
        {
            // Stop the server and return to the main menu
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        DontDestroyOnLoadController.DestroyAll();
    }

    private void HandleHostLeftGame()
    {
        // Perform actions when the host leaves the game
        Debug.Log("Host left the game.");
        // Notify the client or perform any required cleanup
    }
}