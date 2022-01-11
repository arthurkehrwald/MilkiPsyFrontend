using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ReceivedMessageType { Invalid, Feedback, Popup, ChangeStage };

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
    private Dictionary<ReceivedMessageType, HashSet<IReceivedMessageHandler>> receivedMessageHandlers = new Dictionary<ReceivedMessageType, HashSet<IReceivedMessageHandler>>();

    public void SendMessageToServer(string message)
    { 
        if (!IsConnected)
        {
            Debug.LogWarning("[Client] Cannot send message to server because there is no connection");
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

    public bool TryRegisterReceivedMessageHandler(IReceivedMessageHandler handler, ReceivedMessageType messageType)
    {       
        if (!receivedMessageHandlers.ContainsKey(messageType))
        {
            receivedMessageHandlers[messageType] = new HashSet<IReceivedMessageHandler>();
        }

        return receivedMessageHandlers[messageType].Add(handler);
    }

    public bool TryUnregisterReceivedMessageHandler(IReceivedMessageHandler handler, ReceivedMessageType messageType)
    {
        if (receivedMessageHandlers[messageType] == null)
        {
            return false;
        }

        return receivedMessageHandlers[messageType].Remove(handler);
    }

    private async void Start()
    {
        ServerAddress config = ParseConnectionConfig();
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

    private ServerAddress ParseConnectionConfig()
    {
        string path = ConfigPaths.serverAddressPath;

        try
        {
            string text = FileAccessHelper.ReadText(path);
            ServerAddress config = JsonUtility.FromJson<ServerAddress>(text);

            if (!config.IsValid())
            {
                throw new Exception();
            }

            return config;
        }
        catch (Exception)
        {
            string error = string.Format(DebugMessageRelay.FileError, path);
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
        }

        return new ServerAddress
        {
            ip = "127.0.0.1",
            port = 13000
        };
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
                    Debug.LogWarning($"[Client] Error while connecting to server: {e.Message.Split('\r')[0]}" +
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

            bool wasAllPacketDataParsed = ExtractPackets(receivedBytes);
            receivedPacket.Reset(wasAllPacketDataParsed);

            stream.BeginRead(receiveBuffer, 0, ReceiveBufferSize, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Client] Error while receiving message from server: {e.Message}");
            ReconnectToServerAsync();
        }
    }

    private bool ExtractPackets(byte[] data)
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
                ParsePacket(packet);
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

    private void ParsePacket(Packet packet)
    {
        string metaDataJson = packet.ReadString();
        string messageJson = packet.ReadString();

        MessageMetaData metaData;

        try
        {
            metaData = JsonUtility.FromJson<MessageMetaData>(metaDataJson);

            if (!metaData.IsValid())
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            string error = string.Format(DebugMessageRelay.MessageError, metaDataJson, "MetaData");
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
            Debug.LogError("[FeedbackReceiver] Failed to parse message from server");
            return;
        }


        if (metaData.ignoreMessageIfStateOutdated)
        {
            bool isUpToDate = metaData.currentState == StateTracker.Instance.State;
            if (!isUpToDate)
            {
                string msg = "Received outdated message from server. Ignoring";
                DebugMessageRelay.Instance.RelayMessage(msg, DebugMessageType.Info);
                Debug.Log("[FeedbackReceiver] Received outdated message from server. Ignoring");
                return;
            }
        }

        HashSet<IReceivedMessageHandler> handlers;

        bool isAnyHandlerRegistered = receivedMessageHandlers.TryGetValue(metaData.type, out handlers)
            && handlers.Count > 0;

        if (!isAnyHandlerRegistered)
        {
            string nameOfMsgType = metaData.type.ToString();
            string info = $"No response is currently defined for messages of type '{nameOfMsgType}'";
            DebugMessageRelay.Instance.RelayMessage(info, DebugMessageType.Info);
            Debug.Log($"[Client] No handlers registered for message " +
                $"'{messageJson}' of type '{nameOfMsgType}");
            return;
        }

        foreach (IReceivedMessageHandler handler in handlers)
        {
            handler.Handle(messageJson);
        }
    }

    [Serializable]
    private struct ServerAddress : IParseResult
    {
        public string ip;
        public int port;

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            if (port == 0)
            {
                return false;
            }

            return true;
        }
    }

    [Serializable]
    private struct MessageMetaData : IParseResult
    {
        public ReceivedMessageType type;
        public State currentState;
        public bool ignoreMessageIfStateOutdated;

        public bool IsValid()
        {
            if (type == ReceivedMessageType.Invalid)
            {
                return false;
            }

            if (!currentState.IsValid())
            {
                return false;
            }

            return true;
        }
    }
}
