using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeedbackReceiver : Singleton<FeedbackReceiver>
{
    public class FeedbackReceived : UnityEvent<InstructionsOrFeedback, bool> { }

    public FeedbackReceived receveivedFeedback = new FeedbackReceived();

    private void Awake()
    {
        Client.Instance.receivedMessage.AddListener(ReceveivedMessageFromServerHandler);
    }

    private void OnDestroy()
    {
        Client.Instance?.receivedMessage.RemoveListener(ReceveivedMessageFromServerHandler);
    }

    private void ReceveivedMessageFromServerHandler(string message)
    {
        FeedbackMessage parsedMessage;
        try
        {
            parsedMessage = JsonUtility.FromJson<FeedbackMessage>(message);
        }
        catch (ArgumentException)
        {
            // Server sent a message that is not a FeedbackMessage json string
            Debug.LogError("[FeedbackReceiver] Failed to parse message from server");
            return;
        }

        bool isUpToDate = parsedMessage.currentState == StateTracker.Instance.State;

        if (!isUpToDate)
        {
            Debug.Log("[FeedbackReceiver] Received outdated message feedback from server. Ignoring");
            return;
        }

        bool isFeedbackDefined = !string.IsNullOrWhiteSpace(parsedMessage.uniqueFeedbackName);
        if (!isFeedbackDefined)
        {
            receveivedFeedback?.Invoke(null, parsedMessage.goToNextStage);
            return;
        }

        string filename = parsedMessage.uniqueFeedbackName + ".json";
        string feedbackPath = Path.Combine(ConfigPaths.instructionsAndFeedbackPath);

        try
        {
            string feedbackJson = FileAccessHelper.ReadText(feedbackPath);
            InstructionsOrFeedback feedback = JsonUtility.FromJson<InstructionsOrFeedback>(feedbackJson);
            
            if (!feedback.IsValid())
            {
                throw new Exception();
            }

            receveivedFeedback?.Invoke(feedback, parsedMessage.goToNextStage);
        }
        catch (Exception e)
        {
            string error = string.Format(DebugMessageRelay.ReadError, feedbackPath);
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
            Debug.LogError("[FeedbackReceiver] Error reading feedback file " +
                "referenced in message from server: " + e.Message);
        }
    }

    [Serializable]
    private struct FeedbackMessage : IParseResult
    {
        public State currentState;
        public bool goToNextStage;
        public string uniqueFeedbackName;

        public bool IsValid()
        {
            return currentState.IsValid();
        }
    }
}
