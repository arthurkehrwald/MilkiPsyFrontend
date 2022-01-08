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

    public Stage AssociatedStage
    {
        get => associatedStage;
        set
        {
            associatedStage?.stateChanged.RemoveListener(OnAssociatedStageStateChanged);
            associatedStage = value;
            associatedStage.stateChanged.AddListener(OnAssociatedStageStateChanged);
        }        
    }

    private Stage associatedStage;
    private float notRunningHeight;
    private float runningHeight;

    private void Awake()
    {
        notRunningHeight = layoutElement.preferredHeight;
        runningHeight = notRunningHeight * runningHeightMult;
    }

    private void OnDestroy()
    {
        AssociatedStage?.stateChanged.RemoveListener(OnAssociatedStageStateChanged);
    }

    private void OnAssociatedStageStateChanged(StageState state)
    {
        switch (state)
        {
            case StageState.Incomplete:
                layoutElement.preferredHeight = runningHeight;
                rawImage.color = incompleteColor;
                break;
            case StageState.Running:
                layoutElement.preferredHeight = runningHeight;
                rawImage.color = runningColor;
                break;
            case StageState.Complete:
                layoutElement.preferredHeight = runningHeight;
                rawImage.color = completeColor;
                break;
        }
    }
}
