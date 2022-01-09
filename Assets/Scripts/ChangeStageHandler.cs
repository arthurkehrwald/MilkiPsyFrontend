using System;
using UnityEngine;

public class ChangeStageHandler : MonoBehaviour, IReceivedMessageHandler
{
    private const ReceivedMessageType HandledMessageType = ReceivedMessageType.ChangeStage;

    private void OnEnable()
    {
        GameManager.Instance.runningProgramChanged.AddListener(RunningProgramChangedHandler);
    }

    private void OnDisable()
    {
        GameManager.Instance?.runningProgramChanged.RemoveListener(RunningProgramChangedHandler);
    }

    private void RunningProgramChangedHandler(Program runningProgram)
    {
        if (runningProgram == null)
        {
            Client.Instance?.TryUnregisterReceivedMessageHandler(this, HandledMessageType);
        }
        else
        {
            Client.Instance?.TryRegisterReceivedMessageHandler(this, HandledMessageType);
        }
    }

    public void Handle(string messageJson)
    {
        ChangeStageMessageData messageData;
        
        try
        {
            messageData = JsonUtility.FromJson<ChangeStageMessageData>(messageJson);

            if (!messageData.IsValid())
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            Debug.LogError("[ChangeStageHandler] Failed to parse feedback message data");
            string error = string.Format(DebugMessageRelay.MessageError, messageJson, HandledMessageType);
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
            return;
        }

        Program runningProgram = GameManager.Instance.RunningProgram;
        
        if (runningProgram == null)
        {
            Debug.LogError("[ChangeStageHandler] Cannot change stage because no program is running");
            string error = string.Format(DebugMessageRelay.MessageError, messageJson, HandledMessageType);
            DebugMessageRelay.Instance.RelayMessage(error, DebugMessageType.Error);
            return;
        }

        switch (messageData.function)
        {
            case ChangeStageMessageData.Function.Previous:
                runningProgram.GoToPrevStage();
                break;
            case ChangeStageMessageData.Function.Next:
                runningProgram.GoToNextStage();
                break;
            case ChangeStageMessageData.Function.SetIndex:
                runningProgram.RunningStageIndex = messageData.index;
                break;
        }
    }

    [Serializable]
    private struct ChangeStageMessageData : IParseResult
    {
        public enum Function { None, Previous, Next, SetIndex };
        public Function function;
        public int index;

        public bool IsValid()
        {
            if (function == Function.None)
            {
                return false;
            }

            Program runningProgram = GameManager.Instance.RunningProgram;

            if (runningProgram == null)
            {
                return false;
            }

            switch (function)
            {
                case Function.Previous:
                    if (runningProgram.RunningStageIndex == 0)
                    {
                        return false;
                    }
                    break;
                case Function.SetIndex:
                    if (index < 0 || index >= runningProgram.Stages.Count)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
}
