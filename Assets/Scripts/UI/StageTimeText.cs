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
            runningStageDurationMinutes = 0f;
        }
        else
        {
            runningStageStartTime = Time.time;
            runningStageDurationMinutes = runningStage.DurationMinutes;
        }        
    }

    private void Update()
    {
        if (stageTimeText.isActiveAndEnabled)
        {
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
}
