using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreen : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(RunningProgramChangedHandler);

        bool isProgramRunning = GameManager.Instance.RunningProgram != null;
        gameObject.SetActive(isProgramRunning);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.runningProgramChanged.RemoveListener(RunningProgramChangedHandler);
    }

    private void RunningProgramChangedHandler(Program runningProgram)
    {
        bool isProgramRunning = GameManager.Instance.RunningProgram != null;
        gameObject.SetActive(isProgramRunning);
    }
}
