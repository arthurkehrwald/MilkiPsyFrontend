using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Program
{   
    public class RunningProgramChanged : UnityEvent<Program> { };

    public UnityEvent initializationCompleted = new UnityEvent();
    public static RunningProgramChanged runningProgramChanged = new RunningProgramChanged();

    private readonly string programsPath = Application.streamingAssetsPath + "/Configuration/Programs";

    private readonly string uniqueName;
    private string displayName;
    private StageConfig[] stageConfigs;
    private bool isInitializationComplete = false;
    public bool IsInitializationComplete
    {
        get => isInitializationComplete;
        private set
        {
            if (!value || isInitializationComplete)
            {
                return;
            }
            isInitializationComplete = true;
            initializationCompleted?.Invoke();
        }
    }

    private static Program runningProgram;
    public static Program RunningProgram
    {
        get => runningProgram;
        private set
        {
            runningProgram = value; 
            runningProgramChanged?.Invoke(runningProgram);
        }
    }

    /// <summary>
    /// Can throw!
    /// </summary>
    public Program(string jsonName)
    {
        uniqueName = jsonName;

        string jsonPath = programsPath + "/" + jsonName;
        FileAccessHelper.RequestJsonText(jsonPath, (jsonText) =>
        {
            ProgramParseResult parseResult = JsonUtility.FromJson<ProgramParseResult>(jsonText);

            displayName = parseResult.displayName;
            StageConfigParseResult[] stageConfigParseResults = parseResult.stages;
            int stageCount = stageConfigParseResults.Length;
            stageConfigs = new StageConfig[stageCount];
            for (int i = 0; i < stageCount; i++)
            {
                stageConfigs[i].stage = new Stage(stageConfigParseResults[i].stageFilename, this);
                stageConfigs[i].repeats = stageConfigParseResults[i].repeats;
                stageConfigs[i].duration = stageConfigParseResults[i].duration;
            }
            IsInitializationComplete = true;
        });
    }

    public void Start()
    {
        if (!IsInitializationComplete)
        {
            initializationCompleted.AddListener(Start);
            return;
        }

        initializationCompleted.RemoveListener(Start);
        RunningProgram = this;
        stageConfigs[0].stage.Start();
    }

    [System.Serializable]
    private struct ProgramParseResult
    {
        public string displayName;
        public StageConfigParseResult[] stages;
    }

    [System.Serializable]
    private struct StageConfigParseResult
    {
        public string stageFilename;
        public int repeats;
        public float duration;
    }

    private struct StageConfig
    {
        public Stage stage;
        public int repeats;
        public float duration;
    }
}
