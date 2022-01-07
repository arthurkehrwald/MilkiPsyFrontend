using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusSender : Singleton<StatusSender>
{
    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(OnRunningProgramChanged);
        GameManager.Instance.runningStageChanged.AddListener(OnRunningStageChanged);
        Client.Instance.connected.AddListener(OnClientConnected);
    }

    private void OnClientConnected()
    {
        SendStatusToServer();
    }

    private void OnRunningProgramChanged(Program runningProgram)
    {
        SendStatusToServer();
    }

    private void OnRunningStageChanged(Stage runningStage)
    {
        SendStatusToServer();
    }

    private void SendStatusToServer()
    {
        StatusInfo info = new StatusInfo();
        Program runningProgram = GameManager.Instance.RunningProgram;

        if (runningProgram != null)
        {
            info.uniqueProgramName = runningProgram.uniqueName;

            Stage runningStage = GameManager.Instance.RunningProgram.RunningStage;

            if (runningStage != null)
            {
                info.uniqueStageName = runningStage.uniqueName;
            }
        }       

        string statusMessage = JsonUtility.ToJson(info);
        Client.Instance.SendMessageToServer(statusMessage);
    }

    [Serializable]
    private struct StatusInfo
    {
        public string uniqueProgramName;
        public string uniqueStageName;
    }
}
