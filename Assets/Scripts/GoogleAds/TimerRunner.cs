using System;
using UnityEngine;

public class TimerRunner : MonoBehaviour
{
    public static TimerRunner Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RunAfter(float delay, Action action)
    {
        StartCoroutine(RunCoroutine(delay, action));
    }
  
    private System.Collections.IEnumerator RunCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}