using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class RewardPopup : MonoBehaviour, IEventShowRewardPopup
    {
        [SerializeField] Image[] _icons;
        [SerializeField] GameObject _popup;

        private Config _config;
        private EventBus _eventBus;
        private State _state;

        public GameObject Body => _popup.gameObject;

        private void Start()
        {
            _config = GameContext.GetInstance<Config>();
            _eventBus = GameContext.GetInstance<EventBus>();
            _state = GameContext.GetInstance<State>();

            _eventBus.Subscribe<IEventShowRewardPopup>(this);
        }

        public void Show()
        {
            _popup.SetActive(true);
            int currentRoom = _state.CurrentRoom - 1;
            ConfigRoom roomConfig = _config.Rooms[currentRoom];
            for (int i = 0; i < roomConfig.RewardIcons.Length; i++)
            {
                Icon icon = roomConfig.RewardIcons[i];
                Sprite sprite = _config.GetSprite(icon.IconType, icon.Value);
                _icons[i].sprite = sprite;
            }
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventShowRewardPopup>(this);
        }
    }
}
