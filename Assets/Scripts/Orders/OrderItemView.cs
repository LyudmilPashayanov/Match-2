using System;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.Messages;
using MergeIt.Core.WindowSystem;
using MergeIt.Game.Messages;
using MergeIt.Game.Windows.ElementInfo;
using MergeIt.SimpleDI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeIt.Game
{
    public class OrderItemView : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private RectTransform _checkMark;
        [SerializeField] private TextMeshProUGUI _itemCurrentAmountText;
        [SerializeField] private TextMeshProUGUI _itemRequiredAmountText;
        [SerializeField] private Button _infoButton;
        
        private IWindowSystem _windowSystem;
        public ElementConfig ItemType { get; private set; }
        public bool IsDone { get; private set; }

        private int _requiredAmount;
        private int _currentAmount;

        private void Start()
        {
            _windowSystem = DiContainer.Get<IWindowSystem>();
            _infoButton.onClick.AddListener(ItemClicked);
        }

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
            _checkMark.gameObject.SetActive(IsDone);
        }

        private void ItemClicked()
        {
            var infoArgs = new ElementInfoArgs {ElementConfig = ItemType};
            _windowSystem.OpenWindow<ElementInfoPresenter>(enableBlackout: true, args: infoArgs);
        }
    }
}
