using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class Stage
{
    public class RunningStageChanged : UnityEvent<Stage> { };

    public UnityEvent initializationCompleted = new UnityEvent();
    public static RunningStageChanged runningStageChanged = new RunningStageChanged();

    private readonly string stagesPath = Application.streamingAssetsPath + "/Configuration/Stages";
    private readonly string instructionsPath = Application.streamingAssetsPath + "/Configuration/InstructionsAndFeedback";

    public readonly string uniqueName;
    public string DisplayName { get; private set; }
    public InstructionsOrFeedback Instructions { get; private set; }
    public CanBeCompletedBy CanBeCompletedBy { get; private set; }
    public Program ParentProgram { get; private set; }
    public readonly int indexInParentProgram;

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

    private static Stage runningStage;
    public static Stage RunningStage
    {
        get => runningStage;
        private set
        {
            if (value.ParentProgram != Program.RunningProgram)
            {
                return;
            }
            runningStage = value;
            runningStageChanged?.Invoke(runningStage);
        }
    }

    public float TimeElapsed { get; private set; }

    /// <summary>
    /// Can throw!
    /// </summary>    
    public Stage(string fileName, Program parentProgram, int indexInParentProgram)
    {
        uniqueName = fileName;
        ParentProgram = parentProgram;
        this.indexInParentProgram = indexInParentProgram;
        string stagePath = stagesPath + "/" + fileName;
        FileAccessHelper.RequestJsonText(stagePath, (stageJsonText) =>
        {
            StageParseResult parseResult = JsonUtility.FromJson<StageParseResult>(stageJsonText);

            DisplayName = parseResult.displayName;
            string instructionsPath = this.instructionsPath + "/" + parseResult.instructionsFilename;

            FileAccessHelper.RequestJsonText(instructionsPath, (instructionsJsonText) =>
            {
                Instructions = JsonUtility.FromJson<InstructionsOrFeedback>(instructionsJsonText);
                CanBeCompletedBy = parseResult.canBeCompletedBy;
                IsInitializationComplete = true;
            });
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
        RunningStage = this;
    }

    [System.Serializable]
    private struct StageParseResult
    {
        public string displayName;
        public string instructionsFilename;
        public float duration;
        public CanBeCompletedBy canBeCompletedBy;
    }
}

[System.Serializable]
public struct CanBeCompletedBy
{
    public bool userInput;
    public bool evaluation;
    public bool timeout;
}

[System.Serializable]
public struct InstructionsOrFeedback
{
    public string mediaFileName;
    public string text;
}