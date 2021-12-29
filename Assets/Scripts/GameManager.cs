using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RunningProgramChanged : UnityEvent<Program> { }
public class GameManager : Singleton<GameManager>
{
    public RunningProgramChanged runningProgramChanged = new RunningProgramChanged();
    // Pass-through event that is always invoked along with the corresponding
    // event of the running program. Users don't need to worry about
    // changing their subscription when the running program changes
    public RunningStageChanged runningStageChanged = new RunningStageChanged();

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
            runningProgram?.Start();
        }
    }

    private async void Start()
    {
        RunningProgram = await Program.CreateAsync("example_program.json");
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

}
