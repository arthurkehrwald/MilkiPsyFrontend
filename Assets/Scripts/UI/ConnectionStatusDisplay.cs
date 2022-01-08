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
    private Color notConnectedColor;
    [SerializeField]
    private Color connectedColor;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private RawImage background;

    private void Awake()
    {
        text.text = notConnectedText;
        background.color = notConnectedColor;
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
        background.color = connectedColor;
    }

    private void DisconnectedFromServerHandler()
    {
        text.text = notConnectedText;
        background.color = notConnectedColor;
    }
}
