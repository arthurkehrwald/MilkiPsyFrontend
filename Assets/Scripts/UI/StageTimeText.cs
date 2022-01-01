using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class StageTimeText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stageTimeText;

    private float runningStageStartTime;
    private float runningStageDuration;

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(OnRunningStageChanged);
    }

    private void OnRunningStageChanged(Stage runningStage)
    {        
        bool isStageRunning = runningStage != null;
        stageTimeText.gameObject.SetActive(isStageRunning);

        if (runningStage == null)
        {
            runningStageStartTime = 0f;
            runningStageDuration = 0f;
        }
        else
        {
            runningStageStartTime = Time.time;
            runningStageDuration = runningStage.Duration;
        }        
    }

    private void Update()
    {
        if (stageTimeText.isActiveAndEnabled)
        {
            float elapsedSeconds = Time.time - runningStageStartTime;
            float displaySeconds;

            if (runningStageDuration > 0f)
            {
                displaySeconds = runningStageDuration - elapsedSeconds;
            }
            else
            {
                displaySeconds = elapsedSeconds;
            }

            TimeSpan timespan = TimeSpan.FromSeconds(displaySeconds);
            stageTimeText.text = timespan.ToString(@"m\:ss");
        }
    }
}
