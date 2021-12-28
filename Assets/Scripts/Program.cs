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
    private Stage[] stages;
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
            int stageCount = parseResult.stageFilenames.Length;
            stages = new Stage[stageCount];
            for (int i = 0; i < stageCount; i++)
            {
                stages[i]= new Stage(parseResult.stageFilenames[i], this);
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
        stages[0].Start();
    }

    [System.Serializable]
    private struct ProgramParseResult
    {
        public string displayName;
        public string[] stageFilenames;
    }
}
