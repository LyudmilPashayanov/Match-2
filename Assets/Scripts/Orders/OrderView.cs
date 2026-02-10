using System;
using System.Collections.Generic;
using DG.Tweening;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.Messages;
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
        
        [SerializeField] private RectTransform _clickHand;
        
        [SerializeField] private TextMeshProUGUI _completedText;
        [SerializeField] private Button _orderCompleteButton;
        [SerializeField] private Image _orderCompleteButtonImage;
        
        List<OrderItemView> _orderItems = new List<OrderItemView>();

        private OrderDefinition _orderDefinition;
        public OrderDefinition OrderDefinition => _orderDefinition;
        
        private Action<OrderView> _onOrderCompleted;
        private IMessageBus _messageBus;   
        private Tween _handTween;

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
            DisableHelpHand();
            _onOrderCompleted?.Invoke(this);
        }
        
        public void Setup(OrderDefinition orderDefinition, IMessageBus messageBus)
        {
            _messageBus =  messageBus;
            
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
                item.UpdateCurrentAmount(0);
            }

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
            OrderAvailableToServeMessage orderCompletedMessage = new OrderAvailableToServeMessage(){AvailableToServeOrder = this};
            _messageBus.Fire(orderCompletedMessage);
           
            ActivateHelpHand();
           
            _backgroundInProgress.gameObject.SetActive(false);
            _backgroundCompleted.gameObject.SetActive(true);
            _completedText.text = "Completed!";
            _orderCompleteButton.interactable = true;
            _orderCompleteButtonImage.enabled = true;
        }

        private void MarkIncompleted()
        {
            _backgroundInProgress.gameObject.SetActive(true);
            _backgroundCompleted.gameObject.SetActive(false);
            _completedText.text = "Collect:";
            _orderCompleteButton.interactable = false;
            _orderCompleteButtonImage.enabled = false;
        }
        
        private void ActivateHelpHand()
        {
            EnableTutorialHandMessage disableHandMessage = new EnableTutorialHandMessage { Enabled = false};
            _messageBus.Fire(disableHandMessage);
            
            _clickHand.gameObject.SetActive(true);
            _handTween = _clickHand.DOScale(1f, 1f);
            _handTween.SetEase(Ease.InSine);
            _handTween.SetLoops(-1, LoopType.Yoyo);
        } 
        
        private void DisableHelpHand()
        {
            EnableTutorialHandMessage enableHandMessage = new EnableTutorialHandMessage { Enabled = true};
            _messageBus.Fire(enableHandMessage);
            
            _clickHand.gameObject.SetActive(false);
            _handTween?.Kill();
            _handTween = null;
        }
        
        public Vector3 GetCenter()
        {
            return _backgroundCompleted.transform.position;
        }
    }
}
