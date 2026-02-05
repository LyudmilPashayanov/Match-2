using DG.Tweening;
using MergeIt.Core.Saves;
using MergeIt.Core.Services;
using MergeIt.SimpleDI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
    private const float FADE_DURATION = 0.5f;
    
    [SerializeField] private Button _changeSceneButton;
    [SerializeField] private Button _quitGameButton;
    [SerializeField] private Image _fadeBackground;
    private IGameSaveService _saveService;

    private bool _isLoading;
    
    private void Awake()
    {
        _fadeBackground.gameObject.SetActive(true);
        _fadeBackground.color = new Color(0, 0, 0, 1);
        _changeSceneButton.onClick.AddListener(GoToOtherScene);
        if (_quitGameButton != null)
        {
            _quitGameButton.onClick.AddListener(QuitGame);
        }
        FadeOut();
    }

    private void Start()
    {
        _saveService = DiContainer.Get<IGameSaveService>();
    }

    private void FadeOut()
    {
        _fadeBackground.gameObject.SetActive(true);
        _fadeBackground.DOFade(0f, FADE_DURATION).OnComplete(() =>
            {
                _fadeBackground.gameObject.SetActive(false);
            }
        );
    }

    private void QuitGame()
    {
        Application.Quit();
    }
    
    private void GoToOtherScene()
    {
        if (_isLoading) return;
        
        _isLoading = true;
        _saveService.Save(GameSaveType.All);

        int goToIndex = 0;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            goToIndex = 1;
        }
        
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(goToIndex);
        loadOp.allowSceneActivation = false;
        _fadeBackground.gameObject.SetActive(true);

        _fadeBackground.DOFade(1f, FADE_DURATION).OnComplete(() =>
        {
            loadOp.allowSceneActivation = true;
        });
    }
}
