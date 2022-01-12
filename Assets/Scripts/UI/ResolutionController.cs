using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionController : MonoBehaviour
{
    [SerializeField]
    private int minWidth;
    [SerializeField]
    private int minHeight;

#if UNITY_STANDALONE && !UNITY_EDITOR
    private void Awake()
    {
        //Screen.SetResolution(startWidth, startHeight, false);
        MinimumWindowSize.Set(minWidth, minHeight);
    }

    private void OnApplicationQuit()
    {
        MinimumWindowSize.Reset();
    }
#endif
}
