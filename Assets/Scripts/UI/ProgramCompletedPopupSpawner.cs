using UnityEngine;

public class ProgramCompletedPopupSpawner : MonoBehaviour
{
    [SerializeField]
    private RectTransform spawnParent;
    [SerializeField]
    private PopupMessage popupPrefab;

    private void OnEnable()
    {
        GameManager.Instance.runningProgramCompleted.AddListener(RunningProgramCompletedHandler);
    }

    private void OnDisable()
    {
        GameManager.Instance?.runningProgramCompleted.RemoveListener(RunningProgramCompletedHandler);
    }

    private void RunningProgramCompletedHandler()
    {
        SpawnPopup();
    }

    private void SpawnPopup()
    {
        Instantiate(popupPrefab, spawnParent);
    }
}
