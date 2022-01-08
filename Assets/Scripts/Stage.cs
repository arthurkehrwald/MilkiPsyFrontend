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
            string error = string.Format(DebugMessageRelay.ReadError, stagePath);
            throw new Exception(error);
        }

        DisplayName = parseResult.displayName;
        Duration = parseResult.durationSeconds;

        bool hasInstructions = !string.IsNullOrWhiteSpace(parseResult.instructionsFilename);

        if (hasInstructions)
        {
            string instructionsPath = Path.Combine(ConfigPaths.instructionsAndFeedbackPath, parseResult.instructionsFilename);

            try
            {
                string instructionsJsonText = FileAccessHelper.ReadText(instructionsPath);
                Instructions = JsonUtility.FromJson<InstructionsOrFeedback>(instructionsJsonText);

                if (!Instructions.IsValid())
                {
                    throw new Exception();
                }
            }
            catch
            {
                string error = string.Format(DebugMessageRelay.ReadError, instructionsPath);
                throw new Exception(error);
            }
        }
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
    private struct StageParseResult : IParseResult
    {
        public string displayName;
        public string instructionsFilename;
        public float durationSeconds;

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