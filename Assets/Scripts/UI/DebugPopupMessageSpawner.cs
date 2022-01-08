using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPopupMessageSpawner : MonoBehaviour, IDebugMessageHandler
{
    [SerializeField]
    private PopupMessage debugInfoMessagePrefab;
    [SerializeField]
    private PopupMessage debugErrorMessagePrefab;
    [SerializeField]
    private RectTransform spawnedMessagesParent;

    public void HandleDebugMessage(string message, DebugMessageType type)
    {
        PopupMessage prefabToSpawn;

        switch (type)
        {
            case DebugMessageType.Info:
                prefabToSpawn = debugInfoMessagePrefab;
                break;
            case DebugMessageType.Error:
                prefabToSpawn = debugErrorMessagePrefab;
                break;
            default:
                Debug.LogError("[DebugPopupMessageSpawner] Unknown message type");
                return;
        }

        PopupMessage spawnedMessage = Instantiate(prefabToSpawn, spawnedMessagesParent);
        spawnedMessage.Text = message;
    }

    private void Awake()
    {
        DebugMessageRelay.Instance.RegisterHandler(this);
    }

    private void OnDestroy()
    {
        DebugMessageRelay.Instance?.UnregisterHandler(this);
    }
}
