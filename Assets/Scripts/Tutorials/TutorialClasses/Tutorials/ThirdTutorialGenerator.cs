using System;
using DG.Tweening;
using MergeIt.Core.Configs.Types;
using MergeIt.Core.Messages;
using MergeIt.Game.Field;
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
    private FieldLogicModel _fieldLogicModel;
    private const int generatesToFinish = 3;
    private bool _tutorialStarted = false;
    
    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _messageBus.AddListener<MergeElementsMessage>(OnMergeElementMessageHandler);
        _messageBus.AddListener<CreateElementMessage>(OnFieldFullMessageHandler);
        _messageBus.AddListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
    }
    
    private void OnLoadedGameMessageHandler(LoadedGameMessage message)
    {
        _fieldLogicModel = DiContainer.Get<FieldLogicModel>();
        CheckToStart();
    }
    
    private void CheckToStart()
    {
        foreach (var pair in _fieldLogicModel.FieldElements)
        {
            if (pair.Value.InfoParameters.Type == ElementType.Generator)
            {
                StartTutorial();
            }
        }
    }
    
    private void OnMergeElementMessageHandler(MergeElementsMessage message)
    { 
        if (_tutorialStarted)
        {
            EndTutorial();
        }
        if (message.NewElement.InfoParameters.Type == ElementType.Generator)
        {
            StartTutorial();
        }
    }
    
    private void StartTutorial()
    {
        ShowTutorial();
        
        DisableHints();
        _tutorialStarted = true;
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
            EndTutorial();
        }
    }

    private void EndTutorial()
    {
        _handTween.Kill();
        _handTween = null;
        _tutorialStarted = false;
        HideTutorial(FinishTutorial);   
    }
    
    private void DisableHints()
    {
        EnableTutorialHandMessage enableTutorialHandMessage = new EnableTutorialHandMessage(){Enabled = false};
        _messageBus.Fire(enableTutorialHandMessage);
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
        _messageBus.RemoveListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
    }
}
