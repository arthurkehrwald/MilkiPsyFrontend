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
    private float runningStageDurationMinutes;
    private bool isStageRunning = false;

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(OnRunningStageChanged);
    }

    private void OnRunningStageChanged(Stage runningStage)
    {        
        isStageRunning = runningStage != null;

        if (isStageRunning)
        {
            runningStageStartTime = Time.time;
            runningStageDurationMinutes = runningStage.DurationMinutes;
        }
        else
        {
            runningStageStartTime = 0f;
            runningStageDurationMinutes = 0f;
            stageTimeText.text = "00:00";
        }        
    }

    private void Update()
    {
        if (!isStageRunning)
        {
            return;
        }

        float elapsedSeconds = Time.time - runningStageStartTime;
        float displaySeconds;

        if (runningStageDurationMinutes > 0f)
        {
            displaySeconds = runningStageDurationMinutes * 60 - elapsedSeconds;
        }
        else
        {
            displaySeconds = elapsedSeconds;
        }

        TimeSpan timespan = TimeSpan.FromSeconds(displaySeconds);

        if (timespan.Hours > 0)
        {
            stageTimeText.text = timespan.ToString(@"hh\:mm\:ss");
        }
        else
        {
            stageTimeText.text = timespan.ToString(@"mm\:ss");
        }
    }
}
