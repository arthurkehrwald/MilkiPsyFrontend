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
            Refresh();
        }        
    }

    private Stage associatedStage;

    private void OnDestroy()
    {
        AssociatedStage?.stateChanged.RemoveListener(OnAssociatedStageStateChanged);
    }

    private void OnAssociatedStageStateChanged(StageState state)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (associatedStage == null)
        {
            rawImage.color = incompleteColor;
        }

        switch (associatedStage.State)
        {
            case StageState.Incomplete:
                rawImage.color = incompleteColor;
                break;
            case StageState.Running:
                rawImage.color = runningColor;
                break;
            case StageState.Complete:
                rawImage.color = completeColor;
                break;
        }
    }
}
