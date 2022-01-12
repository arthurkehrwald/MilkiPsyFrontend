using System;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackHandler : MonoBehaviour, IReceivedMessageHandler
{
    private const ReceivedMessageType HandledMessageType = ReceivedMessageType.Feedback;

    [SerializeField]
    private bool useAcceptFeedbackButton;
    [SerializeField]
    private InstructionsOrFeedbackDisplay feedbackDisplay;
    [SerializeField]
    private Button acceptFeedbackButton;

    private bool hasUserAcceptedFeedback = false;

    private void Awake()
    {
        Client.Instance.TryRegisterReceivedMessageHandler(this, HandledMessageType);
        GameManager.Instance.runningStageChanged.AddListener(RunningStageChangedHandler);
        if (useAcceptFeedbackButton)
        {
            acceptFeedbackButton.onClick.AddListener(UiAcceptedFeedbackHandler);
        }
    }

    private void OnDestroy()
    {
        Client.Instance?.TryUnregisterReceivedMessageHandler(this, HandledMessageType);
        GameManager.Instance?.runningStageChanged.RemoveListener(RunningStageChangedHandler);
        if (useAcceptFeedbackButton)
        {
            acceptFeedbackButton?.onClick.AddListener(UiAcceptedFeedbackHandler);
        }
    }

    public async void Handle(string messageJson)
    {
        FeedbackMessageData messageData;

        try
        {
            messageData = JsonUtility.FromJson<FeedbackMessageData>(messageJson);

            if (!messageData.IsValid())
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            Debug.LogError("[FeedbackHandler] Failed to parse feedback message data");
            string error = string.Format(DebugMessageRelay.MessageError, messageJson, HandledMessageType);
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
            return;
        }

        InstructionsOrFeedback feedback;

        try
        {
            feedback = InstructionsOrFeedback.ParseFromJson(messageData.jsonFilename);
        }
        catch (Exception e)
        {
            DebugMessageRelay.Instance.RelayMessage(e.Message, DebugMessageType.Error);
            return;
        }

        await feedbackDisplay.Display(feedback, true);
        hasUserAcceptedFeedback = false;

        if (useAcceptFeedbackButton)
        {
            acceptFeedbackButton.gameObject.SetActive(true);
        }
    }

    private async void UiAcceptedFeedbackHandler()
    {
        if (hasUserAcceptedFeedback)
        {
            return;
        }

        hasUserAcceptedFeedback = true;
        acceptFeedbackButton.gameObject.SetActive(false);

        InstructionsOrFeedback currentInstructions = GameManager.Instance.RunningProgram?.RunningStage?.Instructions;
        await feedbackDisplay.Display(currentInstructions);
    }

    private void RunningStageChangedHandler(Stage runningStage)
    {
        if (!hasUserAcceptedFeedback)
        {
            hasUserAcceptedFeedback = true;

            if (useAcceptFeedbackButton)
            {
                acceptFeedbackButton.gameObject.SetActive(false);
            }
        }
    }

    [Serializable]
    private struct FeedbackMessageData : IParseResult
    {
        public string jsonFilename;

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(jsonFilename))
            {
                return false;
            }

            return true;
        }
    }
}
