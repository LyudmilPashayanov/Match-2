using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class RechargeView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _label;
        [SerializeField] Image _progress;

        Config _config;

        private void Start()
        {
            _config = GameContext.GetInstance<Config>();
        }

        public void DisplayTimer(long endDate, IconType iconType)
        {
            if (_config == null)
                _config = GameContext.GetInstance<Config>();

            long currentDelay = endDate - Logic.GetTimestamp();
            long minutes = currentDelay / 60;
            if (minutes > 0)
                _label.text = $"{minutes}:{currentDelay % 60}";
            else
                _label.text = $"{currentDelay % 60}";

            IconTypeConfig delayConfig = _config.GetTypeConfig(iconType);
            float value = (float)currentDelay / (float)delayConfig.EnergyDelay;
            _progress.fillAmount = 1 - value;
        }
    }
}
