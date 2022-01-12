using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageButton : ChangeStageButton
{
    protected override void RunningStageChangedHandler(Stage runningStage)
    {
        bool isProgramComplete = runningStage == null;
        button.interactable = !isProgramComplete;
    }

    protected override void ButtonClickedHandler()
    {
        GameManager.Instance.RunningProgram?.GoToNextStage();
    }
}
