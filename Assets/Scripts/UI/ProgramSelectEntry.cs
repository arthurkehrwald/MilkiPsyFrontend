using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ProgramSelectEntry : MonoBehaviour
{
    public Program AssociatedProgram
    {
        get => associatedProgram;
        set
        {            
            associatedProgram = value;

            if (associatedProgram == null)
            {
                nameText.text = "<null>";
            }
            else
            {
                nameText.text = associatedProgram.DisplayName;
                TimeSpan ts = TimeSpan.FromMinutes(associatedProgram.estimatedDurationMinutes);
                estimatedTimeText.text = string.Format("{0:%h}h {0:%m}m", ts);
            }
        }
    }

    [SerializeField]
    private Button selectButton;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI estimatedTimeText;
    private Program associatedProgram;

    private void Awake()
    {
        GameManager.Instance.programsParsed.AddListener(ProgramsParsedHandler);        
    }

    private void OnEnable()
    {
        selectButton.onClick.AddListener(SelectedHandler);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.programsParsed.RemoveListener(ProgramsParsedHandler);
    }

    private void OnDisable()
    {
        selectButton.onClick.RemoveListener(SelectedHandler);
    }

    private void SelectedHandler()
    {
        GameManager.Instance.RunningProgram = associatedProgram;
    }

    private void ProgramsParsedHandler(IReadOnlyCollection<Program> programs)
    {
        Destroy(gameObject);
    }
}
