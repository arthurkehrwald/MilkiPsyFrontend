using System;
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
        FeedbackMessage parsedMessage = JsonUtility.FromJson<FeedbackMessage>(message);
        bool isUpToDate = parsedMessage.currentState == StateTracker.Instance.State;
        if (isUpToDate)
        {
            string feedbackPath = ConfigPaths.Instance.InstructionsAndFeedbackPath + "/" + parsedMessage.uniqueFeedbackName + ".json";
            string feedbackJson = FileAccessHelper.ReadText(feedbackPath);
            InstructionsOrFeedback feedback = JsonUtility.FromJson<InstructionsOrFeedback>(feedbackJson);
            receveivedFeedback?.Invoke(feedback, parsedMessage.goToNextStage);
        }
    }

    [Serializable]
    private struct FeedbackMessage
    {
        public State currentState;
        public bool goToNextStage;
        public string uniqueFeedbackName;
    }
}
