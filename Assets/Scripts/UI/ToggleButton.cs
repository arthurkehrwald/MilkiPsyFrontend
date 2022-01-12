using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public enum ToggleMode { Toggle, Activate, Deactivate };

    [SerializeField]
    private Button button;
    [SerializeField]
    private GameObject objectToToggle;
    [SerializeField]
    private ToggleMode mode;

    private void OnEnable()
    {
        button.onClick.AddListener(ButtonClickedHandler);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(ButtonClickedHandler);
    }

    private void ButtonClickedHandler()
    {
        switch (mode)
        {
            case ToggleMode.Toggle:
                objectToToggle.SetActive(!objectToToggle.activeSelf);
                break;
            case ToggleMode.Activate:
                objectToToggle.SetActive(true);
                break;
            case ToggleMode.Deactivate:
                objectToToggle.SetActive(false);
                break;
        }
    }
}
