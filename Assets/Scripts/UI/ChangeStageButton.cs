using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ChangeStageButton : MonoBehaviour
{
    [SerializeField]
    protected Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(ButtonClickedHandler);
        GameManager.Instance.runningProgramChanged.AddListener(RunningProgramChangedHandler);
        GameManager.Instance.runningStageChanged.AddListener(RunningStageChangedHandler);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(ButtonClickedHandler);
        GameManager.Instance?.runningProgramChanged.RemoveListener(RunningProgramChangedHandler);
        GameManager.Instance?.runningStageChanged.RemoveListener(RunningStageChangedHandler);
    }

    private void RunningProgramChangedHandler(Program runningProgram)
    {
        bool isProgramRunning = runningProgram == null;
        button.interactable = isProgramRunning;
    }

    protected abstract void RunningStageChangedHandler(Stage runningStage);

    protected abstract void ButtonClickedHandler();
}
