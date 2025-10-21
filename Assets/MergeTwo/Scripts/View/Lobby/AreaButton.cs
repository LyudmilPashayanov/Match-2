using TMPro;
using UnityEngine;

namespace MergeTwo
{
    public class AreaButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _nameLabel;
        [SerializeField] TextMeshProUGUI _progressLabel;
        [SerializeField] ProgressBar _progressBar;

        State _state;
        Config _config;
        EventBus _eventBus;

        private void Start()
        {
            _config = GameContext.GetInstance<Config>();
            _state = GameContext.GetInstance<State>();
            _eventBus = GameContext.GetInstance<EventBus>();

            _progressBar.SetStartPos();
            Display();
        }

        //call from ui
        public void OnClick()
        {
            _eventBus.Emmit<IEventOnAreaButtonClick>(e => e.OnAreaButtonClick());
        }

        public void Display() 
        {
            var data = Logic.Room.GetRoomProgress(_config, _state);
            _nameLabel.text = $"Area {_state.CurrentRoom + 1}";
            _progressLabel.text = $"{data.current}/{data.max}";
            float value = (float)(data.current) / (float)(data.max);
            _progressBar.SetValue(value);
        }
    }
}
