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

    private void Awake()
    {
        if (instance == null)
            instance = this;

        photon = GetComponent<PhotonRealtimeTransport>();
        gameObject.AddToDontDestroyOnLoad();
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
}