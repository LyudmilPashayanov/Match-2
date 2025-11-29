using System;
using System.Collections.Generic;
using MergeIt.Core.Configs.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeIt.Game
{
    public class OrderView : MonoBehaviour
    {
        private const float ITEM_WIDTH = 200;
        [SerializeField] private OrderItemView _orderItemPrefab;
        [SerializeField] private RectTransform _itemsSpawnPoint;
        [SerializeField] private RectTransform _backgroundInProgress;
        [SerializeField] private RectTransform _backgroundCompleted;
        [SerializeField] private TextMeshProUGUI _completedText;
        [SerializeField] private Button _orderCompleteButton;
        
        List<OrderItemView> _orderItems = new List<OrderItemView>();

        private OrderDefinition _orderDefinition;
        public OrderDefinition OrderDefinition => _orderDefinition;
        
        private Action<OrderView> _onOrderCompleted;
        
        public bool IsCompleted { get; private set; }
        
        public void Init(Action<OrderView> OnOrderCompleted)
        {
            MarkIncompleted();
            _onOrderCompleted = OnOrderCompleted;
            _orderCompleteButton.onClick.AddListener(CompleteOrder);
        }

        private void CompleteOrder()
        {
            Debug.Log("COMPLETE ORDER");
            _onOrderCompleted?.Invoke(this);
        }
        
        public void Setup(OrderDefinition orderDefinition)
        {
            _orderDefinition = orderDefinition;
            foreach (var item in orderDefinition.RequiredItems)
            {
                OrderItemView newItem = Instantiate(_orderItemPrefab, _itemsSpawnPoint);
                _orderItems.Add(newItem);
                newItem.GetComponent<RectTransform>().anchoredPosition= new Vector3((_orderItems.Count - 1) * ITEM_WIDTH, 0, 0);
                newItem.Setup(item.Type, item.Amount);
            }
        }

        public void UpdateState(Dictionary<ElementConfig, int> TypeAmounts)
        {
            foreach (var item in _orderItems)
            {
                if (TypeAmounts.TryGetValue(item.ItemType, out int amount))
                {
                    item.UpdateCurrentAmount(amount);
                }
            }
            
            foreach (var item in _orderItems)
            {
                if (item.IsDone == false)
                {
                    MarkIncompleted();
                    IsCompleted = false;
                    return;
                }    
            }
            
            IsCompleted = true;
            MarkCompleted();
        }
        
        private void MarkCompleted()
        {
            _backgroundInProgress.gameObject.SetActive(false);
            _backgroundCompleted.gameObject.SetActive(true);
            _completedText.text = "Completed!";
            _orderCompleteButton.interactable = true;
        }

        private void MarkIncompleted()
        {
            _backgroundInProgress.gameObject.SetActive(true);
            _backgroundCompleted.gameObject.SetActive(false);
            _completedText.text = "Collect:";
            _orderCompleteButton.interactable = false;
        }
    }
}
