// Source: https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient_Daniel : MonoBehaviour
{
	[SerializeField]
	private string serverIpAddress = "localhost";
	[SerializeField]
	private int port = 8052;

	private TcpClient socketConnection;
	private Thread clientReceiveThread;

	void Start()
	{
		ConnectToTcpServer();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SendMessage("Message from Client");
		}
	}

	/// <summary> 	
	/// Set up socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			Debug.LogError("On client connect exception " + e);
		}
	}

	private void DisconnectFromTcpServer()
    {

    }
	
	/// <summary> 	
	/// Runs in background clientReceiveThread. Listens for incoming data. 	
	/// </summary>     
	private void ListenForData()
	{
		while (true)
		{
			try
			{
				socketConnection = new TcpClient(serverIpAddress, port);
				Byte[] bytes = new Byte[1024];
				while (true)
				{
					// Get a stream object for reading 				
					using (NetworkStream stream = socketConnection.GetStream())
					{
						int length;
						// Read incoming stream into byte arrary. 					
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incomingData = new byte[length];
							Array.Copy(bytes, 0, incomingData, 0, length);
							// Convert byte array to string message. 						
							string serverMessage = Encoding.ASCII.GetString(incomingData);
							Debug.Log("Message received: " + serverMessage);
						}
					}
				}
			}
			catch (Exception e)
			{
				bool isNormalShutdown = e is ThreadAbortException;
				if (!isNormalShutdown)
				{
					Debug.LogError(e.Message);
				}
			}
			finally
            {
				socketConnection?.Close();
				Debug.Log("Connection closed");
            }

			// Try to connect every 5 seconds
			Thread.Sleep(5000);
		}
	}

	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	public new bool SendMessage(string msg)
	{
		if (socketConnection == null)		
			return false;		

		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(msg);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				return true;
			}
			return false;
		}
		catch (SocketException socketException)
		{
			Debug.LogError("Socket exception: " + socketException);
			return false;
		}
	}

    private void OnDestroy()
    {
		clientReceiveThread?.Abort();
    }
}