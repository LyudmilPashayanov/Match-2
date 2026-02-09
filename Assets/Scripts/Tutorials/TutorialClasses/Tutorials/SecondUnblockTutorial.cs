using DG.Tweening;
using MergeIt.Core.Messages;
using MergeIt.Game.Field;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class SecondUnblockTutorial : Tutorial
{
    
    private const string FirstTutorialName = "firstMergeTutorial";

    [SerializeField] private RectTransform _hand;
    [SerializeField] private RectTransform _handPos_1;
    [SerializeField] private RectTransform _handPos_2;
    
    private IMessageBus _messageBus;
    private Tween _handTween;
    
    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();

        _messageBus.AddListener<TutorialFinishedMessage>(OnFirstTutorialFinished);
        _messageBus.AddListener<MergeElementsMessage>(OnMergeElementMessageHandler);
    }

    private void OnFirstTutorialFinished(TutorialFinishedMessage message)
    {
        if (message.TutorialFinished.TutorialName == FirstTutorialName)
        {
                ShowTutorial();
                _hand.localPosition = _handPos_1.localPosition;
                _hand.gameObject.SetActive(true);
                _handTween = _hand.DOLocalJump(_handPos_2.localPosition, 100f, 1, 2f);
                _handTween.SetLoops(-1, LoopType.Yoyo);
        }
    }
    
    private void OnMergeElementMessageHandler(MergeElementsMessage message)
    {
        if (message.NewElement.InfoParameters.Name == "Special Pair of Gloves")
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
}
