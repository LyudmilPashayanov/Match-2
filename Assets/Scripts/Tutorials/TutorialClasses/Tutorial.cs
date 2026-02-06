using UnityEngine;

public abstract class Tutorial : MonoBehaviour
{
    [SerializeField] protected TutorialOverlayController TutorialOverlay;

    protected void ShowTutorial()
    {
        TutorialOverlay.gameObject.SetActive(true);
    }
   
    protected void HideTutorial()
    {
        TutorialOverlay.gameObject.SetActive(false);
    }
}
