using System;
using System.Collections;
using UnityEngine;

public class TimerRunner : MonoBehaviour
{
    public static TimerRunner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RunAfter(float delay, Action action)
    {
        StartCoroutine(Run(delay, action));
    }

    private IEnumerator Run(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}