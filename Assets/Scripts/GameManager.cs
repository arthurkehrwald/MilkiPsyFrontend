using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class RunningProgramChanged : UnityEvent<Program> { }
public class ProgramsParsed : UnityEvent<IReadOnlyCollection<Program>> { }
public class GameManager : Singleton<GameManager>
{
    public ProgramsParsed programsParsed = new ProgramsParsed();
    public RunningProgramChanged runningProgramChanged = new RunningProgramChanged();
    // Pass-through events that are always invoked along with the corresponding
    // event of the running program. Users don't need to worry about
    // changing their subscription when the running program changes
    public RunningStageChanged runningStageChanged = new RunningStageChanged();
    public UnityEvent runningProgramCompleted = new UnityEvent();

    private List<Program> programs = new List<Program>();
    public IReadOnlyCollection<Program> Programs
    {
        get => programs.AsReadOnly();
    }
    private Program runningProgram;
    public Program RunningProgram
    {
        get => runningProgram;
        set
        {
            if (value == runningProgram)
            {
                return;
            }

            if (value != null && !programs.Contains(value))
            {
                Debug.LogError("[GameManager] Something attempted to" +
                    "start a program that was not on the list of parsed" +
                    "programs");
                return;
            }
         
            runningProgram?.StopRunning();
            runningProgram?.runningStageChanged.RemoveListener(RunningProgramStageChangedHandler);
            runningProgram?.completed.RemoveListener(RunningProgramCompletedHandler);
            runningProgram = value;
            runningProgramChanged?.Invoke(runningProgram);
            runningProgram?.runningStageChanged.AddListener(RunningProgramStageChangedHandler);
            runningProgram?.completed.AddListener(RunningProgramCompletedHandler);
            runningProgram?.StartRunning();
        }
    }

    private void Awake()
    {
        try
        {
            ParseAllPrograms();
        }
        catch (Exception e)
        {
            DebugMessageRelay.Instance.RelayMessage(e.Message, DebugMessageType.Error);
        }
    }

    private void Update()
    {
        RunningProgram?.UpdateRunning();
    }

    private void RunningProgramStageChangedHandler(Stage runningStage)
    {
        runningStageChanged?.Invoke(runningStage);
    }

    private void RunningProgramCompletedHandler()
    {
        runningProgramCompleted?.Invoke();
    }

    private void ParseAllPrograms()
    {
        string path = ConfigPaths.programFolderPath;
        DirectoryInfo dataDir = new DirectoryInfo(path);

        FileInfo[] directory = dataDir.GetFiles();
        List<Program> tempList = new List<Program>();

        foreach (FileInfo file in directory)
        {
            string name = file.Name;
            bool isJson = Path.GetExtension(name) == ".json";

            if (!isJson)
            {
                continue;
            }

            programs.Add(new Program(name));
        }

        programsParsed?.Invoke(programs.AsReadOnly());
    }
}
