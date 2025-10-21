using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MergeTwo
{
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private Button _levelButton;

        private void Start()
        {
            _levelButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            SceneManager.LoadScene(Constant.GameScene);
        }
    }
}
