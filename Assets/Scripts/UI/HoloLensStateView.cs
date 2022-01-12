using UnityEngine;

public class HoloLensStateView : MonoBehaviour
{
    [SerializeField]
    private RectTransform visuals;

    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(RunningProgramChangedHandler);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.runningProgramChanged.RemoveListener(RunningProgramChangedHandler);
    }

    private void RunningProgramChangedHandler(Program runningProgram)
    {
        bool isProgramRunning = runningProgram != null;
        visuals.gameObject.SetActive(isProgramRunning);
    }
}
