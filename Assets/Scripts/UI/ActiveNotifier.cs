using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActiveNotifier : MonoBehaviour
{
    public class IsActiveChanged : UnityEvent<bool> { }

    public IsActiveChanged isActiveChanged = new IsActiveChanged();

    private void OnEnable()
    {
        isActiveChanged?.Invoke(true);
    }

    private void OnDisable()
    {
        isActiveChanged?.Invoke(false);
    }
}
