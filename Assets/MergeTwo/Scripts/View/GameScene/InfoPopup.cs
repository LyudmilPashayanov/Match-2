using System;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class InfoPopup : MonoBehaviour, IEventIShowInfoPopup
    {
        [SerializeField] GameObject _popup;
        [SerializeField] Item[] _items;
        [SerializeField] Image _canBeFoundIn;

        private EventBus _eventBus;
        private Config _config;

        private void Start()
        {
            _eventBus = GameContext.GetInstance<EventBus>();
            _eventBus.Subscribe<IEventIShowInfoPopup>(this);

            _config = GameContext.GetInstance<Config>();
        }

        public void Show(IconType iconType)
        {
            _popup.SetActive(true);
            IconTypeConfig typeConfig = _config.GetTypeConfig(iconType);
            for (int i = 0; i < typeConfig.Sprites.Count; i++)
            {
                var sprite = typeConfig.Sprites[i];
                _items[i].Body.SetActive(true);
                if (_items[i].Arrow != null)
                {
                    _items[i].Arrow.SetActive(true);
                }
                _items[i].Icon.sprite = sprite;
                _items[i].Icon.SetNativeSize();
                _items[i].Icon.rectTransform.sizeDelta = _items[i].Icon.rectTransform.sizeDelta * 0.5f;
            }

            for (int i = typeConfig.Sprites.Count; i < _items.Length; i++)
            {
                _items[i].Body.SetActive(false);
                if (_items[i].Arrow != null)
                {
                    _items[i].Arrow.SetActive(false);
                }
            }

            IconTypeConfig canBeFoundInConfig = _config.GetTypeConfig(typeConfig.CanBeFoundIn);
            _canBeFoundIn.sprite = canBeFoundInConfig.Sprites[0];
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventIShowInfoPopup>(this);
        }

        [Serializable]
        private class Item
        {
            public GameObject Body;
            public GameObject Arrow;
            public Image Icon;
        }
    }
}
