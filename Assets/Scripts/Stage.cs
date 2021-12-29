using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum StageState { None, Incomplete, Running, Complete }
public class StageStateChanged : UnityEvent<StageState> { }

public class Stage
{
    private readonly string stagesPath = Application.streamingAssetsPath + "/Configuration/Stages";
    private readonly string instructionsPath = Application.streamingAssetsPath + "/Configuration/InstructionsAndFeedback";

    public StageStateChanged stateChanged = new StageStateChanged();
    public readonly string fileName;
    public string DisplayName { get; private set; }
    public InstructionsOrFeedback Instructions { get; private set; }
    public CanBeCompletedBy CanBeCompletedBy { get; private set; }
    public readonly Program parentProgram;
    public readonly int indexInParentProgram;
    public float TimeElapsed { get; private set; }
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
        this.parentProgram = parentProgram;
        parentProgram.runningStageChanged.AddListener(OnParentProgramRunningStageChanged);
        this.indexInParentProgram = indexInParentProgram;
    }

    private async Task<Stage> InitializeAsync()
    { 
        string stagePath = stagesPath + "/" + fileName;
        string stageJsonText = await FileAccessHelper.RequestJsonText(stagePath);

        StageParseResult parseResult = JsonUtility.FromJson<StageParseResult>(stageJsonText);
        DisplayName = parseResult.displayName;
        string instructionsPath = this.instructionsPath + "/" + parseResult.instructionsFilename;
        string instructionsJsonText = await FileAccessHelper.RequestJsonText(instructionsPath);

        Instructions = JsonUtility.FromJson<InstructionsOrFeedback>(instructionsJsonText);
        CanBeCompletedBy = parseResult.canBeCompletedBy;
        return this;
    }

    private void OnParentProgramRunningStageChanged(Stage runningStage)
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

    public void Start()
    {

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