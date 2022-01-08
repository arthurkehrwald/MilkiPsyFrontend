using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStageButton : MonoBehaviour
{
    public enum Effect { NextStage, PreviousStage };

    [SerializeField]
    private Button button;
    [SerializeField]
    private Effect effect;

    private void Awake()
    {
        button.onClick.AddListener(ButtonClickedHandler);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(ButtonClickedHandler);
    }

    private void ButtonClickedHandler()
    {
        switch (effect)
        {
            case Effect.NextStage:
                GameManager.Instance.GoToNextStage();
                break;
            case Effect.PreviousStage:
                GameManager.Instance.GoToPrevStage();
                break;
        }
    }
}
