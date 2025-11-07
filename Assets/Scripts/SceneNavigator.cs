using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
   [SerializeField] private Button _startGameButton;
    private void Start()
    {
        _startGameButton.onClick.AddListener(GoToGame);
    }

    private void GoToGame()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}
