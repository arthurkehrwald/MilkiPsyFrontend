using System;
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
    public readonly float estimatedDurationMinutes;
    public Stage RunningStage
    {
        get => runningStage;
        private set
        {
            if (value == runningStage)
            {
                return;
            }

            if (value != null && !stages.Contains(value))
            {
                Debug.LogError("[Program] Something attempted to" +
                    "start a stage that was not on the list of parsed" +
                    "stages");
                return;
            }

            runningStage?.stateChanged.RemoveListener(OnRunningStageStateChanged);
            runningStage = value;

            if (runningStage == null)
            {
                RunningStageIndex = -1;
            }
            else
            {
                RunningStageIndex = stages.IndexOf(RunningStage);
            }

            runningStageChanged?.Invoke(runningStage);
            runningStage?.stateChanged.AddListener(OnRunningStageStateChanged);
        }
    }
    public int RunningStageIndex
    {
        get => runningStageIndex;
        set
        {
            if (value == runningStageIndex)
            {
                return;
            }

            if (value >= 0 && value < stages.Count)
            {
                runningStageIndex = value;
                RunningStage = stages[runningStageIndex];
            }
            else
            {
                runningStageIndex = -1;
                RunningStage = null;
            }
        }
    }
    public IReadOnlyCollection<Stage> Stages
    {
        get => stages.AsReadOnly();
    }


    private Stage runningStage = null;
    private int runningStageIndex = -1;
    private readonly List<Stage> stages;

    public Program(string fileName)
    {
        this.fileName = fileName;
        uniqueName = Path.GetFileNameWithoutExtension(fileName);
        string jsonPath = Path.Combine(ConfigPaths.programFolderPath, fileName);
        ProgramParseResult parseResult;

        try
        {
            string jsonText = FileAccessHelper.ReadText(jsonPath);
            parseResult = JsonUtility.FromJson<ProgramParseResult>(jsonText);

            if (!parseResult.IsValid())
            {
                throw new Exception();
            }
        }
        catch
        {
            string error = string.Format(DebugMessageRelay.FileError, jsonPath);
            throw new Exception(error);
        }

        DisplayName = parseResult.displayName;
        estimatedDurationMinutes = parseResult.estimatedDurationMinutes;
        stages = new List<Stage>(parseResult.stageFilenames.Length);

        for (int i = 0; i < parseResult.stageFilenames.Length; i++)
        {
            Stage stage = new Stage(parseResult.stageFilenames[i], this, i);
            stages.Add(stage);
        }
    }

    public void StartRunning()
    {
        RunningStageIndex = 0;
    }

    public void UpdateRunning()
    {
        RunningStage?.UpdateRunning();
    }

    public void GoToPrevStage()
    {
        if (RunningStage == null)
        {
            RunningStageIndex = stages.Count - 1;
        }
        else
        {
            RunningStageIndex--;
        }
    }

    public void GoToNextStage()
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

    [Serializable]
    private struct ProgramParseResult : IParseResult
    {
        public string displayName;
        public float estimatedDurationMinutes;
        public string[] stageFilenames;

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return false;
            }

            if (stageFilenames == null)
            {
                return false;
            }

            return true;
        }
    }
}
