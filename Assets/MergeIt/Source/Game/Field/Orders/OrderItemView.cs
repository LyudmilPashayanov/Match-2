using MergeIt.Core.Configs.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeIt.Game
{
    public class OrderItemView : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _itemCurrentAmountText;
        [SerializeField] private TextMeshProUGUI _itemRequiredAmountText;
        
        public ElementConfig ItemType { get; private set; }
        public bool IsDone { get; private set; }

        private int _requiredAmount;
        private int _currentAmount;
        
        public void Setup(ElementConfig type, int requiredAmount)
        {
            ItemType = type;
            _itemIcon.sprite = type.GetIconComponent().GetImage().sprite;
            _requiredAmount = requiredAmount;
            _itemRequiredAmountText.text = requiredAmount.ToString();
            _itemCurrentAmountText.text = "0";
        }

        public void UpdateCurrentAmount(int currentAmount)
        {
            _currentAmount = currentAmount;
            _itemCurrentAmountText.text = currentAmount.ToString();
            if (_currentAmount >= _requiredAmount)
            {
                _itemCurrentAmountText.text = _requiredAmount.ToString();
                IsDone = true;
            }
            else
            {
                IsDone = false;
            }
        } 
    }
}
