using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageProgressBar : MonoBehaviour
{
    [SerializeField]
    private Color incompleteColor;
    [SerializeField]
    private Color runningColor;
    [SerializeField]
    private Color completeColor;
    [SerializeField]
    private float runningHeightMult;

    [SerializeField]
    private RawImage rawImage;
    [SerializeField]
    private LayoutElement layoutElement;

    public int indexOfAssociatedStageInProgram;

    private StageState state;
    public StageState State
    {
        get => state;
        set
        {
            state = value;
            switch (value)
            {
                case StageState.Incomplete:
                    layoutElement.preferredHeight = notRunningHeight;
                    rawImage.color = incompleteColor;
                    break;
                case StageState.Running:
                    layoutElement.preferredHeight = runningHeight;
                    rawImage.color = runningColor;
                    break;
                case StageState.Complete:
                    layoutElement.preferredHeight = notRunningHeight;
                    rawImage.color = completeColor;
                    break;
            }
        }
    }

    private float notRunningHeight;
    private float runningHeight;

    private void Awake()
    {
        notRunningHeight = layoutElement.preferredHeight;
        runningHeight = notRunningHeight * runningHeightMult;

        Stage.runningStageChanged.AddListener(OnRunningStageChanged);
    }

    private void OnRunningStageChanged(Stage runningStage)
    {
        int i = runningStage.indexInParentProgram;
        if (i < indexOfAssociatedStageInProgram)
        {
            State = StageState.Incomplete;
        }
        else if (i == indexOfAssociatedStageInProgram)
        { 
            State = StageState.Running;
        }
        else
        {
            State = StageState.Complete;
        }
    }
}

public enum StageState { Incomplete, Running, Complete }
