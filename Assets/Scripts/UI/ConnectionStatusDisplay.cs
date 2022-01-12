using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectionStatusDisplay : MonoBehaviour
{
    [SerializeField]
    private RectTransform notConnectedObject;
    [SerializeField]
    private RectTransform connectedObject;

    private void Awake()
    {
        bool isConnected = Client.Instance.IsConnected;
        notConnectedObject.gameObject.SetActive(!isConnected);
        connectedObject.gameObject.SetActive(isConnected);

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
        notConnectedObject.gameObject.SetActive(false);
        connectedObject.gameObject.SetActive(true);
    }

    private void DisconnectedFromServerHandler()
    {
        notConnectedObject.gameObject.SetActive(true);
        connectedObject.gameObject.SetActive(false);
    }
}
