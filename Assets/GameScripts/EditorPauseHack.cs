using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class EditorPauseHack : MonoBehaviour
{
    DateTime lastFrameTime;
    public float pauseWhenFrameIsLongerThanSeconds = 2f;

    private void OnEnable()
    {
        lastFrameTime = DateTime.Now;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (DateTime.Now.Subtract(lastFrameTime).CompareTo(TimeSpan.FromSeconds(pauseWhenFrameIsLongerThanSeconds)) > 0)
        {
            if (!EditorApplication.isPaused)
            {
                EditorApplication.isPaused = true;
            }
        }
        lastFrameTime = DateTime.Now;
#endif
    }

    public static void PauseEditor()
    {
#if UNITY_EDITOR
        EditorApplication.isPaused = true;
#endif
    }
}