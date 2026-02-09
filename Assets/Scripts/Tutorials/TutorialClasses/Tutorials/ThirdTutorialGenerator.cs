using System;
using DG.Tweening;
using MergeIt.Core.Configs.Types;
using MergeIt.Core.Messages;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class ThirdTutorialGenerator : Tutorial
{
    [SerializeField] private RectTransform _hand;
    [SerializeField] private RectTransform _handPos_1;
    
    private IMessageBus _messageBus;
    private Tween _handTween;

    private int _generates = 0;
    private const int generatesToFinish = 3;
    
    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _messageBus.AddListener<MergeElementsMessage>(OnMergeElementMessageHandler);
        _messageBus.AddListener<CreateElementMessage>(OnFieldFullMessageHandler);
    }

    private void OnMergeElementMessageHandler(MergeElementsMessage message)
    {
        if (message.NewElement.InfoParameters.Type == ElementType.Generator)
        {
            StartTutorial();
        }
    }
    
    private void StartTutorial()
    {
        ShowTutorial();
        _hand.localPosition = _handPos_1.localPosition;
        _hand.gameObject.SetActive(true);
        _handTween = _hand.DOScale(1.2f, 1f);
        _handTween.SetEase(Ease.InSine);
        _handTween.SetLoops(-1, LoopType.Yoyo);
    }
    
    private void OnFieldFullMessageHandler(CreateElementMessage message)
    {
        _generates++;

        if (_generates == generatesToFinish)
        {
            _handTween.Kill();
            _handTween = null;
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
        _messageBus.RemoveListener<MergeElementsMessage>(OnMergeElementMessageHandler);
        _messageBus.RemoveListener<CreateElementMessage>(OnFieldFullMessageHandler);
    }
}
