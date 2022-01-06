using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class TcpClient_Microsoft : MonoBehaviour
{
    [SerializeField]
    private string serverIp;
    [SerializeField]
    private int port;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage("Message from Client");
        }
    }

    public new void SendMessage(string msg)
    {
        SendMessage(serverIp, port, msg);
    }

    private void SendMessage(String server, int port, String message)
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            TcpClient client = new TcpClient(server, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            Debug.Log("Sent: " + message);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);            
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Debug.Log("Received: " + responseData);

            // Close everything.
            stream.Close();
            client.Close();
        }
        catch (ArgumentNullException e)
        {
            Debug.LogError("ArgumentNullException: " + e);
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e);
        }
    }
}
