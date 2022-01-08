using System;
using UnityEngine;

public class StateSender : Singleton<StateSender>
{
    private void Awake()
    {
        StateTracker.Instance.stateChanged.AddListener(StateChangedHandler);
        Client.Instance.connected.AddListener(ConnectedToServerHandler);
    }

    private void OnDestroy()
    {
        StateTracker.Instance?.stateChanged.RemoveListener(StateChangedHandler);
        Client.Instance?.connected.RemoveListener(ConnectedToServerHandler);
    }

    private void ConnectedToServerHandler()
    {
        SendStateToServer(StateTracker.Instance.State);
    }

    private void StateChangedHandler(State state)
    {
        SendStateToServer(state);
    }

    private void SendStateToServer(State state)
    {
        string statusMessage = JsonUtility.ToJson(state);
        Client.Instance.SendMessageToServer(statusMessage);
    }
}
