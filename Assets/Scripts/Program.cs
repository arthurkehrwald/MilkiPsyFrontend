using System;
using UnityEngine;
using UnityEngine.Events;

public class Program
{   
    public class RunningProgramChanged : UnityEvent<Program> { };

    public UnityEvent initializationCompleted = new UnityEvent();
    public static RunningProgramChanged runningProgramChanged = new RunningProgramChanged();

    private readonly string programsPath = Application.streamingAssetsPath + "/Configuration/Programs";

    public readonly string uniqueName;
    public string DisplayName { get; private set; }
    public Stage[] Stages { get; private set; }

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

            DisplayName = parseResult.displayName;

            Stages = new Stage[parseResult.stageFilenames.Length];
            for (int i = 0; i < Stages.Length; i++)
            {
                Stages[i]= new Stage(parseResult.stageFilenames[i], this, i);
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
        Stages[0].Start();
    }

    [System.Serializable]
    private struct ProgramParseResult
    {
        public string displayName;
        public string[] stageFilenames;
    }
}
