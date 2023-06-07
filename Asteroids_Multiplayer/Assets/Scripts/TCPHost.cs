using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TCPHost : MonoBehaviour
{
    private TcpListener listener;
    private bool isListening = false;
    public string ip;

    private void Start()
    {
        // Start the server when this component is initialized
        StartServer(ip, 12345);
    }

    public void StartServer(string address, int port)
    {
        IPAddress ipAddress = IPAddress.Parse(address);
        listener = new TcpListener(ipAddress, port);
        listener.Start();
        Debug.Log("Server started on port " + port);
        isListening =
            true; // Start the coroutine to listen for incoming connections
        StartCoroutine(AcceptClientsCoroutine());
    }

    private IEnumerator AcceptClientsCoroutine()
    {
        while (isListening)
        {
            if (!listener.Pending())
            {
                yield return null;
            }
            else
            {
                TcpClient client = listener.AcceptTcpClient();
                Debug.Log("Client connected from " + client.Client.RemoteEndPoint.ToString());
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received message: " + message);
                client.Close();
            }
        }
    }

    private void OnDestroy()
    {
        // Stop the server when this component is destroyed
        StopServer();
    }

    public void StopServer()
    {
        isListening = false;
        listener.Stop();
        Debug.Log("Server stopped");
    }
}