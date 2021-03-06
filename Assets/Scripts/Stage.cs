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
    public float DurationMinutes { get; private set; }
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

    public Stage(string fileName, Program parentProgram, int indexInParentProgram)
    {
        this.fileName = fileName;
        uniqueName = Path.GetFileNameWithoutExtension(fileName);
        this.parentProgram = parentProgram;
        this.parentProgram.runningStageChanged.AddListener(ParentProgramRunningStageChangedHandler);
        this.indexInParentProgram = indexInParentProgram;
        string stagePath = Path.Combine(ConfigPaths.stageFolderPath, fileName);
        StageParseResult parseResult;

        try
        {
            string stageJsonText = FileAccessHelper.ReadText(stagePath);
            parseResult = JsonUtility.FromJson<StageParseResult>(stageJsonText);

            if (!parseResult.IsValid())
            {
                throw new Exception();
            }
        }
        catch
        {
            string error = string.Format(DebugMessageRelay.FileError, stagePath);
            throw new Exception(error);
        }

        DisplayName = parseResult.displayName;
        DurationMinutes = parseResult.durationMinutes;

        bool hasInstructions = !string.IsNullOrWhiteSpace(parseResult.instructionsFilename);

        if (hasInstructions)
        {
            // If parse fails, an exception will be thrown that
            // will travel all the way back up the stack to where
            // the parent program constructor was called
            Instructions = InstructionsOrFeedback.ParseFromJson(parseResult.instructionsFilename);
        }
    }

    private void ParentProgramRunningStageChangedHandler(Stage runningStage)
    {
        if (runningStage == null)
        {
            State = StageState.Complete;
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
        if (State == StageState.Running && DurationMinutes > 0f)
        {
            float elapsedSeconds = Time.time - startRunningTime;
            if (elapsedSeconds >= DurationMinutes * 60f)
            {
                State = StageState.Complete;
            }
        }
    }

    [Serializable]
    private struct StageParseResult : IParseResult
    {
        public string displayName;
        public string instructionsFilename;
        public float durationMinutes;

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return false;
            }

            return true;
        }
    }
}