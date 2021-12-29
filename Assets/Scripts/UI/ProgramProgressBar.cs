using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramProgressBar : MonoBehaviour
{
    [SerializeField]
    private StageProgressBar stageProgressBarPrefab;
    [SerializeField]
    private RectTransform stageProgressBarParent;

    private StageProgressBar[] stageProgressBars;

    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(OnRunningProgramChanged);
    }

    private void OnRunningProgramChanged(Program runningProgram)
    {
        if (stageProgressBars != null)
        {
            foreach (StageProgressBar bar in stageProgressBars)
            {
                Destroy(bar.gameObject);
            }
        }

        stageProgressBars = new StageProgressBar[runningProgram.Stages.Length];
        for (int i = 0; i < runningProgram.Stages.Length; i++)
        {
            StageProgressBar.Instantiate(stageProgressBarPrefab, stageProgressBarParent, runningProgram.Stages[i]);
        }
    }
}
