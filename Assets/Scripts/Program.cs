using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunningStageChanged : UnityEvent<Stage> { }

public class Program
{
    public RunningStageChanged runningStageChanged = new RunningStageChanged();
    private readonly string programsPath = Application.streamingAssetsPath + "/Configuration/Programs";
    public readonly string fileName;
    public string DisplayName { get; private set; }
    public List<Stage> Stages { get; private set; }
    private Stage runningStage;
    public Stage RunningStage
    {
        get => runningStage;
        private set
        {
            if (value == runningStage || !Stages.Contains(value))
            {
                return;
            }

            runningStage = value;
            RunningStageIndex = Stages.IndexOf(RunningStage);
            runningStage.Start();
            runningStageChanged?.Invoke(runningStage);
        }
    }
    private int runningStageIndex;
    public int RunningStageIndex
    {
        get => runningStageIndex;
        set
        {
            if (value == runningStageIndex
                || value < 0
                || value >= Stages.Count)
            {
                return;
            }

            runningStageIndex = value;
            RunningStage = Stages[runningStageIndex];
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
    }

    private async Task<Program> InitializeAsync()
    {
        string jsonPath = programsPath + "/" + fileName;
        string jsonText = await FileAccessHelper.RequestJsonText(jsonPath);

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

    public void Start()
    {
        RunningStage = Stages[0];
    }

    public void OnUiGoToPrevStage()
    {
        if (RunningStage == null)
        {
            return;
        }

        RunningStageIndex--;
    }

    public void OnUiGoToNextStage()
    {
        if (RunningStage == null || !RunningStage.CanBeCompletedBy.userInput)
        {
            return;
        }

        RunningStageIndex++;
    }

    [System.Serializable]
    private struct ProgramParseResult
    {
        public string displayName;
        public string[] stageFilenames;
    }
}
