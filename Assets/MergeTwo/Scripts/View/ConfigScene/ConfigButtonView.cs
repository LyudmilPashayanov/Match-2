using System;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class ConfigButtonView : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] Button _button;

        int _index;
        IconType _iconType;
        EventBus _eventBus;

        void Start()
        {
            _button.onClick.AddListener(OnClick);
            _eventBus = GameContext.GetInstance<EventBus>();
        }

        public void Init(Sprite icon, int index, IconType iconType)
        {
            _icon.sprite = icon;
            _index = index;
            _iconType = iconType;
        }

        void OnClick()
        {
            _eventBus.Emmit<IEventSetIconOnField>(e => e.Click(_iconType, _index));
        }
    } 
}
