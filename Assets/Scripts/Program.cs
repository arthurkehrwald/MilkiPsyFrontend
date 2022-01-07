using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunningStageChanged : UnityEvent<Stage> { }

public class Program
{
    public RunningStageChanged runningStageChanged = new RunningStageChanged();
    public readonly string fileName;
    public string DisplayName { get; private set; }
    public readonly string uniqueName;
    public List<Stage> Stages { get; private set; }
    private Stage runningStage = null;
    public Stage RunningStage
    {
        get => runningStage;
        private set
        {
            if (value == runningStage)
            {
                return;
            }

            if (Stages.Contains(value))
            {
                runningStage?.stateChanged.RemoveListener(OnRunningStageStateChanged);
                runningStage = value;
                RunningStageIndex = Stages.IndexOf(RunningStage);
            }
            else if (runningStage == null)
            {
                return;
            }
            else
            {
                runningStage.stateChanged.RemoveListener(OnRunningStageStateChanged);
                runningStage = null;
                RunningStageIndex = -1;
            }

            runningStageChanged?.Invoke(runningStage);
            runningStage?.stateChanged.AddListener(OnRunningStageStateChanged);
        }
    }
    private int runningStageIndex = -1;
    public int RunningStageIndex
    {
        get => runningStageIndex;
        private set
        {
            if (value == runningStageIndex)
            {
                return;
            }

            if (value >= 0 && value < Stages.Count)
            {
                runningStageIndex = value;
                RunningStage = Stages[runningStageIndex];
            }
            else if (runningStageIndex == -1)
            {
                return;
            }
            else
            { 
                runningStageIndex = -1;
                RunningStage = null;
            }
        }
    }

    public static Task<Program> CreateAsync(string jsonName)
    {
        Program program = new Program(jsonName);
        return program.InitializeAsync();
    }

    private Program(string fileName)
    {
        this.fileName = fileName;
        uniqueName = Path.GetFileNameWithoutExtension(fileName);
    }

    private async Task<Program> InitializeAsync()
    {
        string jsonPath = ConfigPaths.Instance.ProgramFolderPath + "/" + fileName;
        string jsonText = await FileAccessHelper.LoadTextAsync(jsonPath);

        ProgramParseResult parseResult = JsonUtility.FromJson<ProgramParseResult>(jsonText);
        DisplayName = parseResult.displayName;
        Stages = new List<Stage>(parseResult.stageFilenames.Length);
        for (int i = 0; i < parseResult.stageFilenames.Length; i++)
        {
            Stage stage = await Stage.CreateAsync(parseResult.stageFilenames[i], this, i);
            Stages.Add(stage);
        }
        return this;
    }

    public void StartRunning()
    {
        RunningStageIndex = 0;
    }

    public void UpdateRunning()
    {
        RunningStage?.UpdateRunning();
    }

    public void OnUiGoToPrevStage()
    {
        if (RunningStage != null)
        {
            RunningStageIndex--;
        }
    }

    public void OnUiGoToNextStage()
    {
        if (RunningStage == null)
        {
            RunningStageIndex = 0;
        }
        else
        {
            RunningStageIndex++;
        }
    }

    private void OnRunningStageStateChanged(StageState state)
    {
        switch (state)
        {
            case StageState.None:
                RunningStageIndex = 0;
                break;
            case StageState.Incomplete:
                RunningStageIndex--;
                break;
            case StageState.Running:
                // Makes no sense and should never happen
                break;
            case StageState.Complete:
                RunningStageIndex++;
                break;
        }
    }

    [System.Serializable]
    private struct ProgramParseResult
    {
        public string displayName;
        public string[] stageFilenames;
    }
}
