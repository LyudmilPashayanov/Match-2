using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class PurchasePiecesPopup : MonoBehaviour,
        IEventOnAreaButtonClick
    {
        [SerializeField] GameObject _popup;
        [SerializeField] Slider _slider;
        [SerializeField] TextMeshProUGUI _progressLabel;
        [SerializeField] PieceBuyItem[] _items;
        [SerializeField] float _progressAnimationDuration = 0.5f;
        [SerializeField] TextMeshProUGUI _title;

        EventBus _eventBus;
        Config _config;
        State _state;
        int _current;

        private void Start()
        {
            _eventBus = GameContext.GetInstance<EventBus>();
            _eventBus.Subscribe<IEventOnAreaButtonClick>(this);

            _config = GameContext.GetInstance<Config>();
            _state = GameContext.GetInstance<State>();

        }

        void IEventOnAreaButtonClick.OnAreaButtonClick()
        {
            _popup.SetActive(true);
            Display();
        }

        private void Display()
        {
            var progress = Logic.Room.GetRoomProgress(_config, _state);
            _progressLabel.text = $"{progress.current}/{progress.max}";
            _slider.value = (float)progress.current / (float)progress.max;
            _current = progress.current;
            _title.text = progress.name;

            var pieces = Logic.Room.GetPiecesToPurchase(_config, _state);
            for (int i = 0; i < _items.Length; i++)
            {
                if (i < pieces.Count)
                {
                    _items[i].gameObject.SetActive(true);
                    _items[i].Display(pieces[i]);
                }
                else
                {
                    _items[i].gameObject.SetActive(false);
                }
            }
        }

        public IEnumerator ProgressAnimation()
        {
            float time = 0;
            var progress = Logic.Room.GetRoomProgress(_config, _state);
            float startValue = (float)_current / (float)progress.max;
            float endValue = (float)progress.current / (float)progress.max;
            while (time < _progressAnimationDuration)
            {
                float value = time / _progressAnimationDuration;
                _slider.value = startValue + (endValue - startValue) * value;
                time += Time.deltaTime;
                yield return null;
            }
            _progressLabel.text = $"{progress.current}/{progress.max}";
        }

        public void ShowPopup() => _popup.SetActive(true);
        public void HidePopup() => _popup.SetActive(false);


        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventOnAreaButtonClick>(this);
        }
    }
}
