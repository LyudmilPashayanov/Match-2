using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class ConfigButtonViewField : MonoBehaviour, IEventRepaintField
    {
        [SerializeField] Image _image;
        [SerializeField] Button _button;
        [SerializeField] Vector2Int _pos;
        [SerializeField] Settings _settings;

        EventBus _eventBus;
        Config _config;
        IconType _iconType = IconType.None;

        public Vector2Int Pos => _pos;

        private void Start()
        {
            _eventBus = GameContext.GetInstance<EventBus>();
            _button.onClick.AddListener(OnButtoClick);
            _config = GameContext.GetInstance<Config>();
            _eventBus.Subscribe<IEventRepaintField>(this);

            UpdateView();
        }

        private void UpdateView()
        {
            if (_config.InitialState.ListField.Count >= 9)
            {
                Icon icon = _config.InitialState.ListField[_pos.x].Icons[_pos.y];
                SetIcon(icon.IconType, icon.Value);
            }
        }

        public void OnButtoClick()
        {
            _image.color = _settings.Selected;
            _eventBus.Emmit<IEventConfigButtonClick>(e => e.Click(this, _pos));
        }

        public void Deselect()
        {
            if (_iconType == IconType.None)
            {
                _image.sprite = null;
                _image.color = new Color(0, 0, 0, 0);
            }
            else
            {
                _image.color = _settings.Default;
            }
        }

        public void SetIcon(IconType iconType, int index)
        {
            _iconType = iconType;
            if (iconType == IconType.None)
            {
                Deselect();
                return;
            }

            Sprite sprite = _config.GetSprite(iconType, index);
            _image.sprite = sprite;
            _image.color = _settings.Default;
        }

        void IEventRepaintField.Repaint()
        {
            UpdateView();
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventRepaintField>(this);
        }

        [Serializable]
        class Settings
        {
            public Color Default;
            public Color Selected;
        }
    }
}
