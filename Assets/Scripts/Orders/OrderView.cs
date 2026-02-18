using System;
using System.Collections.Generic;
using DG.Tweening;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.Configs.Types;
using MergeIt.Core.Helpers;
using MergeIt.Core.Messages;
using MergeIt.Core.Saves;
using MergeIt.Core.Services;
using MergeIt.Game.Effects.Controllers;
using MergeIt.Game.Effects.Parameters;
using MergeIt.Game.Enums;
using MergeIt.Game.Messages;
using MergeIt.Game.Services;
using MergeIt.SimpleDI;
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
        [SerializeField] private List<RectTransform> _experienceStars;
        
        List<OrderItemView> _orderItems = new List<OrderItemView>();

        private OrderDefinition _orderDefinition;
        public OrderDefinition OrderDefinition => _orderDefinition;

        private RectTransform _userExperienceHud;
        private Action<OrderView> _onOrderCompleted;
        private IMessageBus _messageBus;   
        private Tween _handTween;
        
        private UserServiceModel _userServiceModel;
        private IGameSaveService _saveService;
        public bool IsCompleted { get; private set; }
        
        public void Init(Action<OrderView> OnOrderCompleted)
        {
            MarkIncompleted();
            _onOrderCompleted = OnOrderCompleted;
            _orderCompleteButton.onClick.AddListener(CompleteOrder);
            _userServiceModel = DiContainer.Get<UserServiceModel>();
            _saveService = DiContainer.Get<IGameSaveService>();
        }

        private void CompleteOrder()
        {
            Debug.Log("COMPLETE ORDER");
            DisableHelpHand();
            _onOrderCompleted?.Invoke(this);
        }
        
        public void Setup(OrderDefinition orderDefinition, RectTransform userExperienceHud, IMessageBus messageBus)
        {
            _messageBus =  messageBus;
            
            _orderDefinition = orderDefinition;
            
            _userExperienceHud = userExperienceHud;
            
            foreach (var item in orderDefinition.RequiredItems)
            {
                OrderItemView newItem = Instantiate(_orderItemPrefab, _itemsSpawnPoint);
                _orderItems.Add(newItem);
                newItem.GetComponent<RectTransform>().anchoredPosition= new Vector3((_orderItems.Count - 1) * ITEM_WIDTH, 0, 0);
                newItem.Setup(item.Type, item.Amount);
            }
        }
        
        public void AnimateGrantExperience(Action onFinishedAnimation)
        {
            _userServiceModel.Experience.ApplyOperation(ConsumableOperationType.Add,
                _orderDefinition.experienceReward, false);
            _saveService.Save(GameSaveType.User);
            _messageBus.Fire(new ExperienceGainedMessage());
            
            for (int i = 0; i < _experienceStars.Count; i++)
            {
                bool isLast = i == _experienceStars.Count - 1;
                
                _experienceStars[i].localScale = Vector3.zero;
                _experienceStars[i].gameObject.SetActive(true);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_experienceStars[i].DOScale(1, 0.5f));
                sequence.Append(_experienceStars[i].DOMove(_userExperienceHud.position, 0.5f));
                sequence.Insert(1,_experienceStars[i].DOScale(Vector3.zero, 0.5f));
                
                sequence.OnComplete(() =>
                {
                    if (isLast)
                    {
                        onFinishedAnimation?.Invoke();
                    }
                });
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
            if (_handTween is { active: true })
            {
                DisableHelpHand();
            }
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
