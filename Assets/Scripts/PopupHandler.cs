using System;
using System.IO;
using System.Linq;
using UnityEngine;

class PopupHandler : MonoBehaviour, IReceivedMessageHandler
{
    private const ReceivedMessageType HandledMessageType = ReceivedMessageType.Popup;

    [SerializeField]
    private RectTransform spawnedMessagesParent;
    [SerializeField]
    private PopupMessage goodPopupPrefab;
    [SerializeField]
    private PopupMessage badPopupPrefab;
    [SerializeField]
    private PopupMessage neutralPopupPrefab;    

    private void OnEnable()
    {
        Client.Instance.TryRegisterReceivedMessageHandler(this, HandledMessageType);
    }

    private void OnDisable()
    {
        Client.Instance?.TryUnregisterReceivedMessageHandler(this, HandledMessageType);
    }

    public void Handle(string messageJson)
    {
        PopupFile messageData;

        try
        {
            messageData = JsonUtility.FromJson<PopupFile>(messageJson);

            if (!messageData.IsValid())
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            Debug.LogError("[PopupHandler] Failed to parse popup message data");
            string error = string.Format(DebugMessageRelay.MessageError, messageJson, HandledMessageType);
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
            return;
        }

        string popupTextPath = Path.Combine(ConfigPaths.popupMessagesFolderPath, messageData.jsonFileName);
        PopupConfig popupConfig;

        try
        {
            string popupTextJson = FileAccessHelper.ReadText(popupTextPath);
            popupConfig = JsonUtility.FromJson<PopupConfig>(popupTextJson);

            if (!popupConfig.IsValid())
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            Debug.LogError("[PopupHandler] Failed to parse popup message data");
            string error = string.Format(DebugMessageRelay.FileError, popupTextPath);
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
            return;
        }

        SpawnPopup(popupConfig);
    }

    private void SpawnPopup(PopupConfig config)
    {
        PopupMessage prefabToSpawn;

        switch (config.connotation)
        {
            case PopupConfig.Connotation.Good:
                prefabToSpawn = goodPopupPrefab;
                break;
            case PopupConfig.Connotation.Bad:
                prefabToSpawn = badPopupPrefab;
                break;
            case PopupConfig.Connotation.Neutral:
                prefabToSpawn = neutralPopupPrefab;
                break;
            default:
                Debug.LogError("[PopupHandler] No prefab defined for popup connotation");
                return;                
        }

        PopupMessage spawnedMessage = Instantiate(prefabToSpawn, spawnedMessagesParent);
        spawnedMessage.Text = config.text;
    }

    /// <summary>
    /// Sent from the server, references a json file
    /// that contains the text to display in the popup
    /// </summary>
    [Serializable]   
    private struct PopupFile : IParseResult
    {
        public string jsonFileName;
        
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(jsonFileName))
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Parsed from the file referenced in a message from 
    /// the server. Contains information about the popup
    /// </summary>
    [Serializable]
    private struct PopupConfig : IParseResult
    {
        public enum Connotation { None, Good, Neutral, Bad }
        public Connotation connotation;
        public string text;

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(Connotation), connotation))
            {
                return false;
            }

            if (connotation == Connotation.None)
            {
                return false;
            }

            return true;
        }
    }
}