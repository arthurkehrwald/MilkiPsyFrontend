using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class RunningProgramChanged : UnityEvent<Program> { }
public class GameManager : Singleton<GameManager>
{
    public RunningProgramChanged runningProgramChanged = new RunningProgramChanged();
    // Pass-through event that is always invoked along with the corresponding
    // event of the running program. Users don't need to worry about
    // changing their subscription when the running program changes
    public RunningStageChanged runningStageChanged = new RunningStageChanged();

    private List<Program> programs = new List<Program>();
    private Program runningProgram;
    public Program RunningProgram
    {
        get => runningProgram;
        private set
        {
            if (value == runningProgram)
            {
                return;
            }
         
            runningProgram?.runningStageChanged.RemoveListener(OnRunningProgramStageChanged);
            runningProgram = value;
            runningProgramChanged?.Invoke(runningProgram);
            runningProgram?.runningStageChanged.AddListener(OnRunningProgramStageChanged);
            runningProgram?.StartRunning();
        }
    }

    private async void Start()
    {
        //ParseAllPrograms();
        RunningProgram = await Program.CreateAsync("example_program.json");
    }

    private void Update()
    {
        RunningProgram?.UpdateRunning();
    }

    private void OnRunningProgramStageChanged(Stage runningStage)
    {
        runningStageChanged?.Invoke(runningStage);
    }

    public void OnUiGoToPrevStage()
    {
        RunningProgram?.OnUiGoToPrevStage();
    }

    public void OnUiGoToNextStage()
    {
        RunningProgram?.OnUiGoToNextStage();
    }

    private void ParseAllPrograms()
    {
        string path = ConfigPaths.Instance.ProgramFolderPath;
        DirectoryInfo dataDir = new DirectoryInfo(path);

        try
        {
            FileInfo[] fileinfo = dataDir.GetFiles();

            for (int i = 0; i < fileinfo.Length; i++)
            {
                string name = fileinfo[i].Name;
                bool isJson = Path.GetExtension(name) == ".json";

                if (!isJson)
                {
                    continue;
                }

                string text = "";

                using (StreamReader sr = fileinfo[i].OpenText())
                {
                    text = sr.ReadToEnd();
                }

                Debug.Log("name: " + name);
                Debug.Log("text: " + text);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
}
