using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageNameText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(OnRunningStageChanged);
    }

    private void OnRunningStageChanged(Stage runningStage)
    {
        bool isStageRunning = runningStage != null;
        nameText.gameObject.SetActive(isStageRunning);

        if (runningStage != null)
        {
            nameText.text = runningStage.DisplayName;
        }
    }
}
