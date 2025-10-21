using System.Collections;
using TMPro;
using UnityEngine;

namespace MergeTwo
{
    public class TopPanel : MonoBehaviour,
        IEventUpdateTopPanel,
        IEventOrderClaimed
    {
        [SerializeField] TextMeshProUGUI _starLabel;
        [SerializeField] TextMeshProUGUI _energyLabel;
        [SerializeField] Transform _starIcon;
        [SerializeField] Transform _energyIcon;
        [SerializeField] float _additionalScale = 1;
        [SerializeField] float _scaleDuration = 0.5f;
        [SerializeField] float _countSteps = 0.25f;

        State _state;
        EventBus _eventBus;
        int _currentStars;
        int _currentEnergy;

        private void Start()
        {
            _state = GameContext.GetInstance<State>();
            _eventBus = GameContext.GetInstance<EventBus>();
            _eventBus.Subscribe<IEventUpdateTopPanel>(this);
            _eventBus.Subscribe<IEventOrderClaimed>(this);

            _starLabel.text = _state.Stars.ToString();
            _currentStars = _state.Stars;

            _energyLabel.text = _state.Energy.ToString();
            _currentEnergy = _state.Energy;
        }

        public void Display()
        {
            StartCoroutine(Animation());
        }

        void IEventUpdateTopPanel.UpdateTopPanel()
        {
            Display();
        }

        void IEventOrderClaimed.OrderClaimed(int _)
        {
            Display();
        }

        private IEnumerator Animation()
        {
            if (_state.Stars > _currentStars)
            {
                yield return ScaleAnim(_starIcon);
                yield return Count(_starLabel, _currentStars, _state.Stars);
                _currentStars = _state.Stars;
            }
            else
            {
                _starLabel.text = _state.Stars.ToString();
            }

            if (_state.Energy > _currentEnergy)
            {
                yield return ScaleAnim(_energyIcon);
                yield return Count(_energyLabel, _currentEnergy, _state.Energy);
                _currentEnergy = _state.Energy;
            }
            else
            {
                _energyLabel.text = _state.Energy.ToString();
            }
        }

        private IEnumerator ScaleAnim(Transform target)
        {
            float time = 0;
            while (time < _scaleDuration * 0.5f)
            {
                float timeValue = time / (_scaleDuration * 0.5f);
                float value = 1 + _additionalScale * timeValue;
                target.localScale = new Vector3(value, value, 1);
                yield return null;
                time += Time.deltaTime;
            }

            while (time < _scaleDuration)
            {
                float timeValue = 1 - time / _scaleDuration;
                float value = 1 + _additionalScale * timeValue;
                target.localScale = new Vector3(value, value, 1);
                yield return null;
                time += Time.deltaTime;
            }
        }

        private IEnumerator Count(TextMeshProUGUI label, int from, int to) 
        {
            int count = from;
            while (count != to) { 
                label.text = (count + 1).ToString();
                yield return new WaitForSeconds(_countSteps);
                count += 1;
            }
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventUpdateTopPanel>(this);
            _eventBus.UnSubscribe<IEventOrderClaimed>(this);
        }
    }
}
