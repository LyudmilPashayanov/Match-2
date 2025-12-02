using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
    private const float FADE_DURATION = 0.5f;
    
    [SerializeField] private Button _changeSceneButton;
    [SerializeField] private Image _fadeBackground;
  
    private bool _isLoading;
    
    private void Awake()
    {
        _fadeBackground.gameObject.SetActive(true);
        _fadeBackground.color = new Color(0, 0, 0, 1);
        _changeSceneButton.onClick.AddListener(GoToOtherScene);
        FadeOut();
    }

    private void FadeOut()
    {
        _fadeBackground.DOFade(0f, FADE_DURATION);
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

        _fadeBackground.DOFade(1f, FADE_DURATION).OnComplete(() =>
        {
            loadOp.allowSceneActivation = true;
        });
    }
}
