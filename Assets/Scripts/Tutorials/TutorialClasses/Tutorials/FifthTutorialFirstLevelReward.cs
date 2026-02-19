using DG.Tweening;
using MergeIt.Core.Messages;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class FifthTutorialFirstLevelReward : Tutorial
{
    [SerializeField] private RectTransform _stockRectTransform;
    
    private IMessageBus _messageBus;
    private Tween _handTween;


    private bool _tutorialStarted = false; 
    
    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
       _messageBus.AddListener<UpdateStockMessage>(OnStockUpdatedMessageHandler);
       _messageBus.AddListener<StockNotEmptyMessage>(OnStockNotEmptyMessageHandler);
       _messageBus.AddListener<CreateElementMessage>(OnStockItemClickedMessageHandler);
    }

    private void OnStockNotEmptyMessageHandler(StockNotEmptyMessage obj)
    {
        StartTutorial();
    }

    private void OnStockUpdatedMessageHandler(UpdateStockMessage obj)
    {
        StartTutorial();
    }
    
    private void StartTutorial()
    {
        _tutorialStarted = true;
        
        DisableHints();
        
        _mainCanvasRaycaster.enabled = false;
        
        ShowTutorial();
        
        TutorialOverlay.FocusOn(_stockRectTransform, Vector2.zero);
    }
    
    private void OnStockItemClickedMessageHandler(CreateElementMessage obj)
    {
        if (_tutorialStarted)
        {
            _mainCanvasRaycaster.enabled = true;
            _tutorialStarted = false;
            HideTutorial(FinishTutorial);
        }
    }
    
    private void DisableHints()
    {
        EnableTutorialHandMessage disableTutorialHandMessage = new EnableTutorialHandMessage(){Enabled = false} ;
        _messageBus.Fire(disableTutorialHandMessage);
    }
    
    private void FinishTutorial()
    {
        TutorialFinishedMessage tutorialFinishedMessage = new TutorialFinishedMessage(){TutorialFinished = this} ;
        _messageBus.Fire(tutorialFinishedMessage);
    }

    private void OnDisable()
    {
        _messageBus.RemoveListener<UpdateStockMessage>(OnStockUpdatedMessageHandler);
        _messageBus.RemoveListener<StockNotEmptyMessage>(OnStockNotEmptyMessageHandler);
        _messageBus.RemoveListener<CreateElementMessage>(OnStockItemClickedMessageHandler);
    }
}
