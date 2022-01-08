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

    public Stage associatedStage;

    private float notRunningHeight;
    private float runningHeight;

    public static StageProgressBar Instantiate(StageProgressBar prefab, RectTransform parent, Stage associatedStage)
    {
        GameObject gameObj = Instantiate(prefab.gameObject, parent);
        StageProgressBar bar = gameObj.GetComponent<StageProgressBar>();
        bar.notRunningHeight = bar.layoutElement.preferredHeight;
        bar.runningHeight = bar.notRunningHeight * bar.runningHeightMult;
        bar.associatedStage = associatedStage;
        bar.associatedStage.stateChanged.AddListener(bar.OnAssociatedStageStateChanged);
        return bar;
    }

    private void Awake()
    {     
        associatedStage?.stateChanged.AddListener(OnAssociatedStageStateChanged);
    }

    private void OnDestroy()
    {
        associatedStage?.stateChanged.RemoveListener(OnAssociatedStageStateChanged);
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
