// Source: https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer_Daniel : MonoBehaviour
{
	[SerializeField]
	private int port = 8052;
	/// <summary> 	
	/// TCPListener to listen for incoming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener;
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;

	void Start()
	{
		// Start TcpServer background thread 		
		tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Start();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SendMessage("Message from Server");
		}
	}

	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incoming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncomingRequests()
	{
		try
		{
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse("192.168.2.105"), port);
			tcpListener.Start();
			Debug.Log("Server is listening");
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						int length;
						// Read incoming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incomingData = new byte[length];
							Array.Copy(bytes, 0, incomingData, 0, length);
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incomingData);
							Debug.Log("Message received: " + clientMessage);
						}
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}

	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	public new bool SendMessage(string msg)
	{
		if (connectedTcpClient == null)		
			return false;		

		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream();
			if (stream.CanWrite)
			{				
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(msg);
				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
				return true;
			}
			return false;
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
			return false;
		}
	}
}