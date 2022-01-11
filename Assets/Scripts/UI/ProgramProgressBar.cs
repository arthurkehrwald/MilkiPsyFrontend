using System.Linq;
using UnityEngine;

public class ProgramProgressBar : MonoBehaviour
{
    [SerializeField]
    private StageProgressBar stageProgressBarPrefab;
    [SerializeField]
    private RectTransform stageProgressBarParent;

    private StageProgressBar[] stageProgressBars;

    bool isRefreshScheduled = false;    

    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(OnRunningProgramChanged);
        Refresh();
    }

    private void OnDestroy()
    {
        GameManager.Instance?.runningProgramChanged.RemoveListener(OnRunningProgramChanged);
    }

    private void OnEnable()
    {
        if (isRefreshScheduled)
        {
            Refresh();
        }
    }

    private void OnRunningProgramChanged(Program runningProgram)
    {
        if (isActiveAndEnabled)
        {
            Refresh();
        }
        else
        {
            isRefreshScheduled = true;
        }
    }

    private void Refresh()
    {
        isRefreshScheduled = false;

        if (stageProgressBars != null)
        {
            foreach (StageProgressBar bar in stageProgressBars)
            {
                if (bar != null)
                {
                    Destroy(bar.gameObject);
                }
            }
        }

        Program runningProgram = GameManager.Instance.RunningProgram;

        if (runningProgram == null)
        {
            return;
        }

        stageProgressBars = new StageProgressBar[runningProgram.Stages.Count];
        for (int i = 0; i < stageProgressBars.Length; i++)
        {
            stageProgressBars[i] = Instantiate(stageProgressBarPrefab, stageProgressBarParent);
            stageProgressBars[i].AssociatedStage = runningProgram.Stages.ElementAt(i);
        }
    }
}
