using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum StageState { None, Incomplete, Running, Complete }
public class StageStateChanged : UnityEvent<StageState> { }

public class Stage
{
    public StageStateChanged stateChanged = new StageStateChanged();
    public readonly string fileName;
    public readonly string uniqueName;
    public string DisplayName { get; private set; }
    public InstructionsOrFeedback Instructions { get; private set; }
    public readonly Program parentProgram;
    public readonly int indexInParentProgram;
    public float Duration { get; private set; }
    private float startRunningTime;
    private StageState state;
    public StageState State
    {
        get => state;
        private set
        {
            if (value == state)
            {
                return;
            }

            state = value;
            startRunningTime = state == StageState.Running ? Time.time : 0f;
            stateChanged?.Invoke(state);
        }
    }

    public static Task<Stage> CreateAsync(string fileName, Program parentProgram, int indexInParentProgram)
    {
        Stage stage = new Stage(fileName, parentProgram, indexInParentProgram);
        return stage.InitializeAsync();
    }

    private Stage(string fileName, Program parentProgram, int indexInParentProgram)
    {
        this.fileName = fileName;
        uniqueName = Path.GetFileNameWithoutExtension(fileName);
        this.parentProgram = parentProgram;
        this.parentProgram.runningStageChanged.AddListener(ParentProgramRunningStageChangedHandler);
        this.indexInParentProgram = indexInParentProgram;
    }

    private async Task<Stage> InitializeAsync()
    { 
        string stagePath = ConfigPaths.Instance.StageFolderPath + "/" + fileName;
        string stageJsonText = await FileAccessHelper.LoadTextAsync(stagePath);

        StageParseResult parseResult = JsonUtility.FromJson<StageParseResult>(stageJsonText);
        DisplayName = parseResult.displayName;
        Duration = parseResult.durationSeconds;
        string instructionsPath = ConfigPaths.Instance.InstructionsAndFeedbackPath + "/" + parseResult.instructionsFilename;
        string instructionsJsonText = await FileAccessHelper.LoadTextAsync(instructionsPath);

        Instructions = JsonUtility.FromJson<InstructionsOrFeedback>(instructionsJsonText);
        return this;
    }

    private void ParentProgramRunningStageChangedHandler(Stage runningStage)
    {
        if (runningStage == null)
        {
            State = StageState.Incomplete;
            return;
        }

        int i = runningStage.indexInParentProgram;
        if (i == indexInParentProgram)
        {
            State = StageState.Running;
        }
        else if (i < indexInParentProgram)
        {
            State = StageState.Incomplete;
        }
        else
        {
            State = StageState.Complete;
        }
    }

    public void UpdateRunning()
    {
        if (State == StageState.Running && Duration > 0f)
        {
            float elapsedTime = Time.time - startRunningTime;
            if (elapsedTime >= Duration)
            {
                State = StageState.Complete;
            }
        }
    }

    [Serializable]
    private struct StageParseResult
    {
        public string displayName;
        public string instructionsFilename;
        public float durationSeconds;
        public bool canBeCompletedByUserInput;
    }
}