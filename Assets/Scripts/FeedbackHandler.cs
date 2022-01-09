using System;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackHandler : MonoBehaviour, IReceivedMessageHandler
{
    private const ReceivedMessageType HandledMessageType = ReceivedMessageType.Feedback;

    [SerializeField]
    private InstructionsOrFeedbackDisplay feedbackDisplay;
    [SerializeField]
    private Button acceptFeedbackButton;

    private bool hasUserAcceptedFeedback = false;

    private void OnEnable()
    {
        Client.Instance.TryRegisterReceivedMessageHandler(this, HandledMessageType);
        GameManager.Instance.runningStageChanged.AddListener(RunningStageChangedHandler);
        acceptFeedbackButton.onClick.AddListener(UiAcceptedFeedbackHandler);
    }

    private void OnDisable()
    {
        Client.Instance?.TryUnregisterReceivedMessageHandler(this, HandledMessageType);
        GameManager.Instance?.runningStageChanged.RemoveListener(RunningStageChangedHandler);
        acceptFeedbackButton?.onClick.AddListener(UiAcceptedFeedbackHandler);
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

        await feedbackDisplay.Display(feedback);
        hasUserAcceptedFeedback = false;
        acceptFeedbackButton.gameObject.SetActive(true);
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
            acceptFeedbackButton.gameObject.SetActive(false);
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
