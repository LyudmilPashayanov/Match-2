using System;
using DG.Tweening;
using MergeIt.Core.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class FourthFirstOrderTutorial : Tutorial
{
    [SerializeField] private RectTransform _hand;
    
    private IMessageBus _messageBus;
    private Tween _handTween;

    private int _orderReadyId;

    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _messageBus.AddListener<OrderAvailableToServeMessage>(OnOrderAvailableToServeMessageHandler);
        _messageBus.AddListener<OrderCompletedMessage>(OnOrderCompletedMessageHandler);
    }

    private void OnOrderAvailableToServeMessageHandler(OrderAvailableToServeMessage message)
    {
        StartTutorial(message);
    }
    
    private void StartTutorial(OrderAvailableToServeMessage message)
    {
        _orderReadyId = message.AvailableToServeOrder.OrderDefinition.OrderId;
        _mainCanvasRaycaster.enabled = false;
        ShowTutorial();
        _hand.position = message.AvailableToServeOrder.GetCenter();
        _hand.gameObject.SetActive(true);
        _handTween = _hand.DOScale(1.2f, 1f);
        _handTween.SetEase(Ease.InSine);
        _handTween.SetLoops(-1, LoopType.Yoyo);
        TutorialOverlay.FocusOn(message.AvailableToServeOrder.GetComponent<RectTransform>(), Vector2.zero);
    }
    
    private void OnOrderCompletedMessageHandler(OrderCompletedMessage message)
    {
        if (message.CompletedOrder.OrderDefinition.OrderId == _orderReadyId)
        {
            Debug.Log("Order completed");
            _mainCanvasRaycaster.enabled = true;
            HideTutorial(FinishTutorial);
        }
    }

    private void FinishTutorial()
    {
        TutorialFinishedMessage tutorialFinishedMessage = new TutorialFinishedMessage(){TutorialFinished = this} ;
        _messageBus.Fire(tutorialFinishedMessage);
    }

    private void OnDisable()
    {
        _messageBus.RemoveListener<OrderAvailableToServeMessage>(OnOrderAvailableToServeMessageHandler);
        _messageBus.RemoveListener<OrderCompletedMessage>(OnOrderCompletedMessageHandler);
    }
}
