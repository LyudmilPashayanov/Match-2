using UnityEngine;

public abstract class Tutorial : MonoBehaviour
{
    protected string TutorialName;
    
    [SerializeField] protected TutorialOverlayController TutorialOverlay;

    protected void ShowTutorial()
    {
        TutorialOverlay.gameObject.SetActive(true);
    }
   
    protected void HideTutorial()
    {
        TutorialOverlay.gameObject.SetActive(false);
    }

    public void SetTutorialName(string nameOfTutorial)
    {
        TutorialName = nameOfTutorial;
    }
}
