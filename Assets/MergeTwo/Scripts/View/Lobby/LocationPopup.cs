using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class LocationPopup : MonoBehaviour, IEventBottomPanel
    {
        [SerializeField] GameObject _popup;
        [SerializeField] Image[] _roomsIcons; 
        
        EventBus _eventBus;
        Config _config;
        State _state;

        private void Start()
        {
            _eventBus = GameContext.GetInstance<EventBus>();
            _eventBus.Subscribe<IEventBottomPanel>(this);
            _config = GameContext.GetInstance<Config>();
            _state = GameContext.GetInstance<State>();
        }

        public void OnAreaClick()
        {
            Display();
            _popup.SetActive(true);
        }

        public void OnHomeClick()
        {
            _popup.SetActive(false);
        }

        private void Display() 
        {
            for (int i = 0; i < _roomsIcons.Length; i++)
            {
                _roomsIcons[i].sprite = Logic.Room.GetLocationImage(i, _state, _config);
            }
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventBottomPanel>(this);
        }
    }
}
