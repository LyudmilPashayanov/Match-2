using System.Collections.Generic;
using DG.Tweening;
using MergeIt.Core.Configs.Types;
using MergeIt.Core.Messages;
using MergeIt.Game;
using MergeIt.Game.Field;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;
using UnityEngine.UI;

public class SixthMergeGeneratorsTutorial : Tutorial
{
    private const int GENERATORS_NEEDED = 2;
   
    private IMessageBus _messageBus;

    [SerializeField] private RectTransform _hand;
    [SerializeField] private Image _handImage;
    private Sequence _handTweenSeq;


    private FieldLogicModel _fieldLogicModel;
    private const int generatesToFinish = 3;
    private bool _tutorialStarted = false;
    
    [SerializeField] private List<RectTransform> generatorsOnBoard = new List<RectTransform>(); 

    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _messageBus.AddListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
    }

    private void OnElementOnBoardMessageHandler(ElementOnBoardMessage obj)
    {
        CheckToStart();
    }

    private void OnLoadedGameMessageHandler(LoadedGameMessage message)
    {
        _fieldLogicModel = DiContainer.Get<FieldLogicModel>();
        _messageBus.AddListener<MergeElementsMessage>(OnMergeElementMessageHandler);
        _messageBus.AddListener<ElementOnBoardMessage>(OnElementOnBoardMessageHandler);
        CheckToStart();
    }
    
    private void CheckToStart()
    {
        int generatorsFound = 0;
        generatorsOnBoard.Clear();
        foreach (var pair in _fieldLogicModel.FieldElements)
        {
            if (pair.Value.InfoParameters.Type == ElementType.Generator)
            {
                generatorsFound++;
                _fieldLogicModel.CellComponents.TryGetValue(pair.Value.InfoParameters.LogicPosition, out FieldCellComponent generatorObject);
                if (pair.Value.InfoParameters.IsBlocked == false && pair.Value.InfoParameters.IsInvisibleBlocked == false && generatorObject)
                {
                    generatorsOnBoard.Add(generatorObject.FieldElementPresenter.RectTransform);
                }
                
                if (generatorsFound == GENERATORS_NEEDED)
                {
                    StartTutorial();
                    return;
                }
            }
        }
    }
    
    private void OnMergeElementMessageHandler(MergeElementsMessage message)
    { 
        if (_tutorialStarted && message.NewElement.InfoParameters.Type == ElementType.Generator)
        {
            EndTutorial();
        }
    }
    
    private void StartTutorial()
    {
        ShowTutorial();

        DisableHints();

        TutorialOverlay.FocusOn(generatorsOnBoard[0],generatorsOnBoard[1], Vector2.zero);
        
        Vector3 objectPos_1 = generatorsOnBoard[0].position;
        Vector3 objectPos_2 = generatorsOnBoard[1].position;
        StartHandLoop(objectPos_1, objectPos_2);
        _tutorialStarted = true;

    }

    private void EndTutorial()
    {
        StopHandLoop();
        _tutorialStarted = false;
        HideTutorial(FinishTutorial);   
    }
    
    private void StartHandLoop(Vector3 cand1, Vector3 cand2)
    {
        _hand.gameObject.SetActive(true);
        _hand.position = cand1;
        _handTweenSeq = DOTween.Sequence();
        _handTweenSeq.Append(_handImage.DOFade(1, 0.2f));
        _handTweenSeq.Append(_hand.DOMove(cand2, 1f));
        _handTweenSeq.Append(_handImage.DOFade(0, 0.2f));
        _handTweenSeq.AppendInterval(1f);
        _handTweenSeq.SetLoops(-1, LoopType.Restart);
    }

    private void StopHandLoop()
    {
        _handTweenSeq?.Kill();
        _handTweenSeq = null;
        _handImage.DOFade(0, 0.1f);
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
        _messageBus.RemoveListener<ElementOnBoardMessage>(OnElementOnBoardMessageHandler);
        _messageBus.RemoveListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
    }
}
