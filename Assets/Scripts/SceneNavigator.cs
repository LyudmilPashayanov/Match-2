using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
    private const float FADE_DURATION = 0.5f;
    
    [SerializeField] private Button _changeSceneButton;
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
  
    private bool _isLoading;
    
    private void Start()
    {
        _changeSceneButton.onClick.AddListener(GoToOtherScene);
    }
    
    private void GoToOtherScene()
    {
        if (_isLoading) return;
        
        _isLoading = true;

        int goToIndex = 0;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            goToIndex = 1;
        }
        
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(goToIndex);
        loadOp.allowSceneActivation = false;

        _fadeCanvasGroup.DOFade(0f, FADE_DURATION).OnComplete(() =>
        {
            loadOp.allowSceneActivation = true;
        });
    }
}
