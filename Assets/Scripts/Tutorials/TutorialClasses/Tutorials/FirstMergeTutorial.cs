using DG.Tweening;
using MergeIt.Core.Messages;
using MergeIt.Game.Field;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class FirstMergeTutorial : Tutorial
{
    [SerializeField] private RectTransform _hand;
    [SerializeField] private RectTransform _handPos_1;
    [SerializeField] private RectTransform _handPos_2;
    
    private IMessageBus _messageBus;
    private FieldLogicModel _fieldLogicModel;
    
    private Tween _handTween;
    
    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _fieldLogicModel = DiContainer.Get<FieldLogicModel>();

        _messageBus.AddListener<LoadedGameMessage>(OnGameLoadedMessageHandler);
        _messageBus.AddListener<MergeElementsMessage>(OnMergeElementMessageHandler);
    }

    private void OnGameLoadedMessageHandler(LoadedGameMessage message)
    {
        foreach (var pair in _fieldLogicModel.FieldElements)
        {
            if (pair.Value.InfoParameters.Name == "Glove")
            {
                TutorialInProgressMessage tutorialInProgressMessage = new TutorialInProgressMessage(){TutorialCurrentlyInProgressName = TutorialName};
                _messageBus.Fire(tutorialInProgressMessage);
                
                ShowTutorial();
                _hand.localPosition = _handPos_1.localPosition;
                _hand.gameObject.SetActive(true);
                _handTween = _hand.DOLocalJump(_handPos_2.localPosition, 100f,1,2f);
                _handTween.SetLoops(-1, LoopType.Yoyo);
                break;
            }
        }
    }
   
    private void OnMergeElementMessageHandler(MergeElementsMessage message)
    {
        if (message.NewElement.InfoParameters.Name == "Pair of Gloves")
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
        _messageBus?.RemoveListener<LoadedGameMessage>(OnGameLoadedMessageHandler);
        _messageBus?.RemoveListener<MergeElementsMessage>(OnMergeElementMessageHandler);
    }
}