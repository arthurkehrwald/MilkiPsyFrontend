using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectionStatusDisplay : MonoBehaviour
{
    [SerializeField]
    private string notConnectedText = "Establishing Connection...";
    [SerializeField]
    private string connectedText = "Connected to Server";
    [SerializeField]
    private TextMeshProUGUI text;

    private void Awake()
    {
        text.text = Client.Instance.IsConnected ? connectedText : notConnectedText;
        Client.Instance.connected.AddListener(ConnectedToServerHandler);
        Client.Instance.disconnected.AddListener(DisconnectedFromServerHandler);
    }

    private void OnDestroy()
    {
        Client.Instance?.connected.RemoveListener(ConnectedToServerHandler);
        Client.Instance?.disconnected.RemoveListener(DisconnectedFromServerHandler);
    }

    private void ConnectedToServerHandler()
    {
        text.text = connectedText;
    }

    private void DisconnectedFromServerHandler()
    {
        text.text = notConnectedText;
    }
}
