using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgramNameDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    private void Awake()
    {
        Program.runningProgramChanged.AddListener(OnRunningProgramChanged);
    }

    private void OnRunningProgramChanged(Program runningProgram)
    {
        nameText.text = runningProgram.DisplayName;
    }
}
