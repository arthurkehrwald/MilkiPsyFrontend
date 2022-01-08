using UnityEngine.Events;

public class StateChanged : UnityEvent<State> { }
public class StateTracker : Singleton<StateTracker>
{
    public StateChanged stateChanged = new StateChanged();
    public State State
    {
        get => state;
        set
        {
            if (value == state)
            {
                return;
            }

            state = value;
            stateChanged?.Invoke(state);
        }
    }

    private State state;

    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(RunningProgramChangedHandler);
        GameManager.Instance.runningStageChanged.AddListener(RunningStageChangedHandler);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.runningProgramChanged.RemoveListener(RunningProgramChangedHandler);
        GameManager.Instance?.runningStageChanged.RemoveListener(RunningStageChangedHandler);
    }

    private void RunningProgramChangedHandler(Program runningProgram)
    {
        UpdateState(runningProgram, runningProgram.RunningStage);
    }

    private void RunningStageChangedHandler(Stage runningStage)
    {
        UpdateState(GameManager.Instance.RunningProgram, runningStage);
    }

    private void UpdateState(Program runningProgram, Stage runningStage)
    {
        State state = new State();

        if (runningProgram != null)
        {
            state.uniqueProgramName = runningProgram.uniqueName;

            if (runningStage != null)
            {
                state.uniqueStageName = runningStage.uniqueName;
            }
        }

        State = state;
    }
}
