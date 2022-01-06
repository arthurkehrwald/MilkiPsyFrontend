using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadScheduler : MonoBehaviour
{
    private static readonly List<Action> executeOnMainThread = new List<Action>();
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
    private static bool hasPendingActions = false;

    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="_action">The action to be executed on the main thread.</param>
    public static void ScheduleAction(Action _action)
    {
        if (_action == null)
        {
            Console.WriteLine("[MainThreadScheduler] Cannot schedule execution of null action");
            return;
        }

        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(_action);
            hasPendingActions = true;
        }
    }

    private void Update()
    {
        ExecutePendingActions();
    }

    /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
    private static void ExecutePendingActions()
    {
        if (hasPendingActions)
        {
            executeCopiedOnMainThread.Clear();
            lock (executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread);
                executeOnMainThread.Clear();
                hasPendingActions = false;
            }

            for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
            {
                executeCopiedOnMainThread[i]();
            }
        }
    }
}