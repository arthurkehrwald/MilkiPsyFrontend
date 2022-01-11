using UnityEngine;
using UnityEngine.UI;

public class RestartProgramButton : MonoBehaviour
{
    [SerializeField]
    private Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(OnClickedHandler);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickedHandler);
    }

    private void OnClickedHandler()
    {
        Program runningProgram = GameManager.Instance.RunningProgram;

        if (runningProgram != null)
        {
            runningProgram.RunningStageIndex = 0;
        }
    }
}
