using UnityEngine;
using UnityEngine.UI;

public class QuitProgramButton : MonoBehaviour
{
    [SerializeField]
    private Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(ClickHandler);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(ClickHandler);
    }

    private void ClickHandler()
    {
        GameManager.Instance.RunningProgram = null;
    }
}
