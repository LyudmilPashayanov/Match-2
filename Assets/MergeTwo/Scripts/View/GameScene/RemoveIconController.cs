using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class RemoveIconController : MonoBehaviour, IEventIconMerged
    {
        [SerializeField] Grabber _grabber;
        [SerializeField] InputController _input;
        [SerializeField] Transform _frame;
        [SerializeField] Image _viewIcon;
        [SerializeField] Button _removeButton;
        [SerializeField] GameObject _removePanel;

        Icon _currentIcon;
        State _state;
        Config _config;
        Vector2 _frameStartPos;
        EventBus _eventBus;

        private void Start()
        {
            _state = GameContext.GetInstance<State>();
            _config = GameContext.GetInstance<Config>();
            _eventBus = GameContext.GetInstance<EventBus>();
            _removeButton.onClick.AddListener(OnRemoveClick);
            _frameStartPos = _frame.position;
            _removePanel.SetActive(false);

            _eventBus.Subscribe<IEventIconMerged>(this);
        }

        private void OnRemoveClick()
        {
            if (_currentIcon != null)
            {
                _frame.position = _frameStartPos;
                Logic.RemoveIcon(_state, _currentIcon.Pos);
                _eventBus.Emmit<IEventFieldUpdate>(e => e.UpdateView());
                _currentIcon = null;
                _removePanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (_input.IsTapDown)
            {
                for (int i = 0; i < _grabber.Icons.Count; i++)
                {
                    IconView view = _grabber.Icons[i];
                    if (view.Icon.IconType != IconType.None && view.IsInside(_input.CursorPos))
                    {
                        _frame.position = view.transform.position;
                        _currentIcon = view.Icon;
                        _viewIcon.sprite = _config.GetSprite(_currentIcon.IconType, _currentIcon.Value);
                        _viewIcon.SetNativeSize();
                        _viewIcon.rectTransform.sizeDelta = _viewIcon.rectTransform.sizeDelta * 0.5f;
                        _removePanel.SetActive(true);
                    }
                }
            }
        }

        void IEventIconMerged.IconMerged(Icon icon)
        {
            _currentIcon = null;
            _viewIcon.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventIconMerged>(this);
        }
    }
}
