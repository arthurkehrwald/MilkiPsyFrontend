using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class RunningStageChanged : UnityEvent<Stage> { }
public class Program
{
    public RunningStageChanged runningStageChanged = new RunningStageChanged();
    private readonly string programsPath = Application.streamingAssetsPath + "/Configuration/Programs";
    public readonly string fileName;
    public string DisplayName { get; private set; }
    public Stage[] Stages { get; private set; }
    private Stage runningStage;
    public Stage RunningStage
    {
        get => runningStage;
        private set
        {
            if (value == runningStage)
            {
                return;
            }

            runningStage = value;
            runningStage.Start();
            runningStageChanged?.Invoke(runningStage);
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
        Stages = new Stage[parseResult.stageFilenames.Length];
        for (int i = 0; i < Stages.Length; i++)
        {
            Stages[i] = await Stage.CreateAsync(parseResult.stageFilenames[i], this, i);
        }
        return this;
    }

    public void Start()
    {
        RunningStage = Stages[0];
    }

    [System.Serializable]
    private struct ProgramParseResult
    {
        public string displayName;
        public string[] stageFilenames;
    }
}
