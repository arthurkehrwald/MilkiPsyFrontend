using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Client : Singleton<Client>
{
    public class ReceivedMessage : UnityEvent<string> { }

    private const int ReceiveBufferSize = 4096;
    private const int SendBufferSize = 4096;

    [SerializeField]
    private int retryConnectDelayMs = 3000;

    public UnityEvent connected = new UnityEvent();
    public ReceivedMessage receivedMessage = new ReceivedMessage();
    public UnityEvent disconnected = new UnityEvent();

    private bool isConnected = false;
    public bool IsConnected
    {
        get => isConnected;
        private set
        { 
            if (value == isConnected)
            {
                return;
            }

            isConnected = value;

            if (isConnected)
            {
                MainThreadScheduler.ScheduleAction(() =>
                {
                    connected?.Invoke();
                });
            }
            else
            {
                MainThreadScheduler.ScheduleAction(() =>
                {
                    disconnected?.Invoke();
                });
            }
        }
    }

    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer;
    private readonly Packet receivedPacket = new Packet();
    private CancellationTokenSource connectCancellation = new CancellationTokenSource();
    private string serverIp;
    private int serverPort;

    public void SendMessageToServer(string message)
    {
        if (!IsConnected)
        {
            Debug.LogError("[Client] Cannot send message to server because it is not connected");
            return;
        }

        using Packet packet = new Packet();
        packet.Write(message);
        packet.WriteLength();

        try
        {
            stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            Debug.Log($"[Client] Sent message to server: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Client] Error sending message to server: {e.Message}");
        }
    }

    private async void Start()
    {
        ConnectionConfig config = ParseConnectionConfig();
        serverIp = config.ip;
        serverPort = config.port;
        await ConnectToServerAsync(serverIp, serverPort, connectCancellation.Token);
    }

    private void OnApplicationQuit()
    {
        if (IsConnected)
        {
            DisconnectFromServer();
        }
        else
        {
            connectCancellation.Cancel();
        }
    }

    private ConnectionConfig ParseConnectionConfig()
    {
        FileInfo fileInfo = new FileInfo(ConfigPaths.Instance.ServerAddressPath);
        using StreamReader reader = fileInfo.OpenText();
        string text = reader.ReadToEnd();
        ConnectionConfig config = JsonUtility.FromJson<ConnectionConfig>(text);
        return config;
    }

    private async Task ConnectToServerAsync(string ip, int port, CancellationToken cancellationToken)
    {
        if (IsConnected)
        {
            Debug.LogError("[Client] Cannot connect to server, because it is already connected");
            return;
        }

        client = new TcpClient
        {
            ReceiveBufferSize = ReceiveBufferSize,
            SendBufferSize = SendBufferSize
        };
        stream = null;
        receiveBuffer = new byte[ReceiveBufferSize];
        receivedPacket.Reset(true);

        while (!client.Connected)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                await client.ConnectAsync(ip, port);
            }
            catch (Exception e)
            {
                if (e is SocketException || e is ObjectDisposedException)
                {
                    Debug.LogError($"[Client] Error while connecting to server: {e.Message.Split('\r')[0]}" +
                        $" Will try again in {retryConnectDelayMs}ms");
                    await Task.Delay(retryConnectDelayMs);
                }
                else
                {
                    throw e;
                }
            }
        }

        IsConnected = true;
        stream = client.GetStream();
        stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);

        Debug.Log("[Client] Successfully connected to server");
    }

    private void DisconnectFromServer()
    {
        if (!IsConnected)
        {
            Debug.LogError("[Client] Cannot disconnect from server because client is not connected");
            return;
        }

        IsConnected = false;

        client.Close();
        stream = null;
        receivedPacket.Reset();
        receiveBuffer = null;
        client = null;

        Debug.Log("[Client] Disconnected from server");
    }

    public async void ReconnectToServerAsync()
    {
        DisconnectFromServer();
        await ConnectToServerAsync(serverIp, serverPort, connectCancellation.Token);
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        if (!IsConnected)
        {
            return;
        }

        try
        {
            int receivedByteCount = stream.EndRead(result);
            if (receivedByteCount == 0)
            {
                ReconnectToServerAsync();
                return;
            }

            byte[] receivedBytes = new byte[receivedByteCount];
            Array.Copy(receiveBuffer, receivedBytes, receivedBytes.Length);

            bool wasAllPacketDataParsed = ParsePacketData(receivedBytes);
            receivedPacket.Reset(wasAllPacketDataParsed);

            stream.BeginRead(receiveBuffer, 0, ReceiveBufferSize, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Client] Error while receiving message from server: {e.Message}");
            ReconnectToServerAsync();
        }
    }

    private bool ParsePacketData(byte[] data)
    {
        int packetLength = 0;

        receivedPacket.SetBytes(data);

        if (receivedPacket.UnreadLength() >= 4)
        {
            packetLength = receivedPacket.ReadInt();
            if (packetLength <= 0)
            {
                return true;
            }
        }

        while (packetLength > 0 && packetLength <= receivedPacket.UnreadLength())
        {
            byte[] packetBytes = receivedPacket.ReadBytes(packetLength);
            MainThreadScheduler.ScheduleAction(() =>
            {
                using Packet packet = new Packet(packetBytes);
                string message = packet.ReadString();
                receivedMessage?.Invoke(message);
                Debug.Log($"[Client] Received message from server: {message}");
            });

            packetLength = 0;

            if (receivedPacket.UnreadLength() >= 4)
            {
                packetLength = receivedPacket.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }
        }

        return packetLength <= 1;
    }

    [Serializable]
    private struct ConnectionConfig
    {
        public string ip;
        public int port;
    }
}
