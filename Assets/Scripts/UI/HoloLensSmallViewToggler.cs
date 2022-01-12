using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using System.Collections;

public class HoloLensSmallViewToggler : MonoBehaviour
{
    [SerializeField]
    private float appearVerticalPos = 0f;
    [SerializeField]
    private RadialView smallView;
    [SerializeField]
    private RectTransform visuals;

    private float enabledVerticalPos;
    private Coroutine disableSmallViewCoroutine;

    private void Awake()
    {
        GameManager.Instance.runningProgramChanged.AddListener(RunningProgramChangedHandler);
        enabledVerticalPos = smallView.FixedVerticalPosition;
        smallView.FixedVerticalPosition = appearVerticalPos;
    }

    private void OnDestroy()
    {
        GameManager.Instance?.runningProgramChanged.RemoveListener(RunningProgramChangedHandler);
    }

    private void RunningProgramChangedHandler(Program runningProgram)
    {        
        if (disableSmallViewCoroutine != null)
        {
            StopCoroutine(disableSmallViewCoroutine);
            disableSmallViewCoroutine = null;
        }
        bool isProgramRunning = runningProgram != null;
        visuals.gameObject.SetActive(isProgramRunning);

        if (isProgramRunning)
        {
            smallView.gameObject.SetActive(true);
            smallView.FixedVerticalPosition = enabledVerticalPos;
        }
        else
        {
            smallView.FixedVerticalPosition = appearVerticalPos;
            disableSmallViewCoroutine = StartCoroutine(DisableSmallViewAfterSeconds(2f));
        }
    }

    private IEnumerator DisableSmallViewAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        smallView.gameObject.SetActive(false);
    }
}
