using System;
using UnityEngine;

public abstract class Tutorial : MonoBehaviour
{
    protected string TutorialName;
    
    [SerializeField] protected TutorialOverlayController TutorialOverlay;

    protected void ShowTutorial()
    {
        TutorialOverlay.AnimateIn();
    }
   
    protected void HideTutorial(Action onComplete = null)
    {
        TutorialOverlay.AnimateOut(onComplete);
    }

    public void SetTutorialName(string nameOfTutorial)
    {
        TutorialName = nameOfTutorial;
    }
}
