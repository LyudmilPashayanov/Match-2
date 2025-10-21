using System;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class BottomHomePanel : MonoBehaviour
    {
        [SerializeField] Button _homeButton;
        [SerializeField] Button _areaButton;
        [SerializeField] Sprite _active;
        [SerializeField] Sprite _pressed;

        EventBus _eventBus;

        private void Start()
        {
            _homeButton.onClick.AddListener(OnHomeClick);
            _areaButton.onClick.AddListener(OnAreaClick);
            _eventBus = GameContext.GetInstance<EventBus>();
        }

        private void OnAreaClick()
        {
            _homeButton.image.sprite = _active;
            _areaButton.image.sprite = _pressed;
            _eventBus.Emmit<IEventBottomPanel>(e => e.OnAreaClick());
        }

        private void OnHomeClick()
        {
            _homeButton.image.sprite = _pressed;
            _areaButton.image.sprite = _active;
            _eventBus.Emmit<IEventBottomPanel>(e => e.OnHomeClick());
        }
    }
}
