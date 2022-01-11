using UnityEngine;

public class ProgramCompletedPopupSpawner : MonoBehaviour
{
    [SerializeField]
    private RectTransform spawnParent;
    [SerializeField]
    private PopupMessage popupPrefab;

    private void OnEnable()
    {
        GameManager.Instance.runningStageChanged.AddListener(RunningStageChangedHandler);
    }

    private void OnDisable()
    {
        GameManager.Instance?.runningStageChanged.RemoveListener(RunningStageChangedHandler);
    }

    private void RunningStageChangedHandler(Stage runningStage)
    {
        if (runningStage == null)
        {
            SpawnPopup();
        }
    }

    private void SpawnPopup()
    {
        Instantiate(popupPrefab, spawnParent);
    }
}
