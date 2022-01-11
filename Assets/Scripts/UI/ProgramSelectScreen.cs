using System.Collections.Generic;
using UnityEngine;

public class ProgramSelectScreen : MonoBehaviour
{
    [SerializeField]
    private ProgramSelectEntry selectEntryPrefab;
    [SerializeField]
    private RectTransform spawnParent;
    [SerializeField]
    private RectTransform screenRoot;

    private List<ProgramSelectEntry> spawnedPrefabs = new List<ProgramSelectEntry>();
    private bool isRefreshScheduled = false;

    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(RunningProgramChangedHandler);
        GameManager.Instance.programsParsed.AddListener(ProgramsParsedHandler);

        bool isProgramRunning = GameManager.Instance.RunningProgram != null;
        screenRoot.gameObject.SetActive(!isProgramRunning);

        if (!isProgramRunning)
        {
            Refresh();
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance?.runningProgramChanged.RemoveListener(RunningProgramChangedHandler);
        GameManager.Instance?.programsParsed.RemoveListener(ProgramsParsedHandler);

    }

    private void OnEnable()
    {
        if (isRefreshScheduled)
        {
            Refresh();
        }
    }

    private void ProgramsParsedHandler(IReadOnlyCollection<Program> programs)
    {
        if (screenRoot.gameObject.activeInHierarchy)
        {
            Refresh();
        }
        else
        {
            isRefreshScheduled = true;
        }
    }

    private void RunningProgramChangedHandler(Program runningProgram)
    {
        bool isProgramRunning = runningProgram != null;
        screenRoot.gameObject.SetActive(!isProgramRunning);
    }

    private void Refresh()
    {
        isRefreshScheduled = false;

        foreach(ProgramSelectEntry entry in spawnedPrefabs)
        {
            Destroy(entry.gameObject);
        }

        foreach (Program program in GameManager.Instance.Programs)
        {
            ProgramSelectEntry entry = Instantiate(selectEntryPrefab, spawnParent);
            entry.AssociatedProgram = program;
            spawnedPrefabs.Add(entry);
        }
    }
}
