using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageNameDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(OnRunningStageChanged);
    }

    private void OnRunningStageChanged(Stage runningStage)
    {
        nameText.text = runningStage.DisplayName;
    }
}
