using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousStageButton : ChangeStageButton
{
    protected override void RunningStageChangedHandler(Stage runningStage)
    {
        bool isFirstStage = false;
        
        if (runningStage != null && runningStage.indexInParentProgram == 0)
        {
            isFirstStage = true;
        }

        button.interactable = !isFirstStage;
    }

    protected override void ButtonClickedHandler()
    {
        GameManager.Instance.RunningProgram.GoToPrevStage();
    }
}
