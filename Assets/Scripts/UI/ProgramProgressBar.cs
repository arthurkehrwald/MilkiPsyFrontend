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

        stageProgressBars = new StageProgressBar[runningProgram.Stages.Count];
        for (int i = 0; i < stageProgressBars.Length; i++)
        {
            stageProgressBars[i] = Instantiate(stageProgressBarPrefab, stageProgressBarParent);
            stageProgressBars[i].AssociatedStage = runningProgram.Stages[i];
        }
    }
}
