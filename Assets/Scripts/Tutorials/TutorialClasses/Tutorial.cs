using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tutorial : MonoBehaviour
{
    public string TutorialName { private set; get; }
    
    [SerializeField] protected TutorialOverlayController TutorialOverlay;
    [SerializeField] protected GraphicRaycaster _mainCanvasRaycaster; // Tutorials have to be in correct order from first to last!!!

    protected void ShowTutorial()
    {
        TutorialOverlay.AnimateIn();
    }
   
    protected void HideTutorial(Action onComplete = null)
    {
        _mainCanvasRaycaster.enabled = false;
        onComplete+=()=>_mainCanvasRaycaster.enabled = true;
        TutorialOverlay.AnimateOut(onComplete);
    }

    public void SetTutorialName(string nameOfTutorial)
    {
        TutorialName = nameOfTutorial;
    }
}
