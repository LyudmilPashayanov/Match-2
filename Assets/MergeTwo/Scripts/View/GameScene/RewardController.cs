using UnityEngine;

namespace MergeTwo
{
    public class RewardController : MonoBehaviour
    {
        [SerializeField] GameObject _rewardBody;
        [SerializeField] SpriteRenderer _icon;
        [SerializeField] TapController _tapController;
        [SerializeField] FlyIconAnimator _flyIconAnimator;

        private State _state;
        private Config _config;

        private void Start()
        {
            _state = GameContext.GetInstance<State>();
            _config = GameContext.GetInstance<Config>();
            _tapController.OnClick += OnIconClick;

            TryToDisplay();
        }

        private void TryToDisplay()
        {
            if (_state.IconToTake.Count > 0)
            {
                _icon.sprite = _config.GetSprite(_state.IconToTake[0].IconType, _state.IconToTake[0].Value);
                _rewardBody.SetActive(true);
            }
            else
            {
                _rewardBody.SetActive(false);
            }
        }

        private void OnIconClick()
        {
            Icon targetIcon = Logic.CollectIcon(_state);
            TryToDisplay();
            _flyIconAnimator.StartAnimation(_icon.transform.position, targetIcon.Pos, targetIcon.IconType, targetIcon.Value);
        }
    }
}
