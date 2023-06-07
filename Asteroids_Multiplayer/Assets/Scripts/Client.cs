using System;
using System.Net;
using System.Net.Sockets;


public class Client

{
    public static void Main()

    {
        // Define the server IP address and port

        IPAddress ipAddress = IPAddress.Parse("192.168.178.112");

        int port = 12345;


        // Create a TCP client object and connect to the server

        TcpClient client = new TcpClient();

        client.Connect(ipAddress, port);

        Console.WriteLine("Connected to server");


        // Send a message to the server and receive a response

        NetworkStream stream = client.GetStream();

        string message = "Hello, server!";

        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(message);

        stream.Write(buffer, 0, buffer.Length);

        Console.WriteLine("Sent message: " + message);

        client.Close();
    }
}