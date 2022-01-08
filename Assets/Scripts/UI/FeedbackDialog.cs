using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FeedbackDialog : MonoBehaviour
{
    [SerializeField]
    private InstructionsOrFeedbackDisplay feedbackDisplay;
    [SerializeField]
    private Button acceptFeedbackButton;

    private bool hasUserAcceptedFeedback = false;
    private bool goNextStageWhenUserAccepts = false;

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(RunningStageChangedHandler);
        FeedbackReceiver.Instance.receveivedFeedback.AddListener(ReceivedFeedbackHandler);
        acceptFeedbackButton.onClick.AddListener(UiAcceptedFeedbackHandler);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.runningStageChanged.RemoveListener(RunningStageChangedHandler);
        FeedbackReceiver.Instance?.receveivedFeedback.RemoveListener(ReceivedFeedbackHandler);
        acceptFeedbackButton?.onClick.AddListener(UiAcceptedFeedbackHandler);
    }

    private async void ReceivedFeedbackHandler(InstructionsOrFeedback feedback, bool goToNextStage)
    {
        if (feedback == null)
        {
            if (goToNextStage)
            {
                GameManager.Instance.GoToNextStage();
            }

            return;
        }

        await feedbackDisplay.Display(feedback);
        goNextStageWhenUserAccepts = goToNextStage;
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
        if (goNextStageWhenUserAccepts)
        {
            GameManager.Instance.GoToNextStage();
        }
        else
        {
            InstructionsOrFeedback currentInstructions = GameManager.Instance.RunningProgram?.RunningStage?.Instructions;
            await feedbackDisplay.Display(currentInstructions);
        }
    }

    private void RunningStageChangedHandler(Stage runningStage)
    {
        if (!hasUserAcceptedFeedback)
        {
            hasUserAcceptedFeedback = true;
            acceptFeedbackButton.gameObject.SetActive(false);
        }
    }
}
