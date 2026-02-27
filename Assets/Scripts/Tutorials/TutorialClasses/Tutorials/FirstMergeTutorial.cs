using System.Collections.Generic;
using DG.Tweening;
using MergeIt.Core.Messages;
using MergeIt.Game.Field;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;
using UnityEngine.UI;

public class FirstMergeTutorial : Tutorial
{
    [SerializeField] private RectTransform _hand;
    [SerializeField] private RectTransform _handPos_1;
    [SerializeField] private RectTransform _handPos_2;
    [SerializeField] private Button _storyScreen;
    [SerializeField] private List<RectTransform> _storyScreens;
    
    private IMessageBus _messageBus;
    private FieldLogicModel _fieldLogicModel;
    
    private Tween _handTween;
    private int currentStory = -1;

    private void Start()
    {
        _messageBus = DiContainer.Get<IMessageBus>();
        _fieldLogicModel = DiContainer.Get<FieldLogicModel>();

        _messageBus.AddListener<LoadedGameMessage>(OnGameLoadedMessageHandler);
        _messageBus.AddListener<MergeElementsMessage>(OnMergeElementMessageHandler);
        _storyScreen.onClick.AddListener(StartNextStory);
    }
    
    private const float HAND_ANIMATION_DURATION = 1f;
    private const float RESTART_DELAY = 0.5f;

    private void OnGameLoadedMessageHandler(LoadedGameMessage message)
    {
        foreach (var pair in _fieldLogicModel.FieldElements)
        {
            if (pair.Value.InfoParameters.Name == "Glove")
            {
                EnableTutorialHandMessage enableTutorialHandMessage = new EnableTutorialHandMessage(){Enabled = false};
                _messageBus.Fire(enableTutorialHandMessage);
                StartNextStory();
                break;
            }
        }
    }

    private void StartNextStory()
    {
        _storyScreen.gameObject.SetActive(true);
        currentStory++;
        
        if (currentStory > 0)
        {
            _storyScreens[currentStory - 1].gameObject.SetActive(false);    
        }
        if (currentStory == _storyScreens.Count)
        {
            _storyScreen.gameObject.SetActive(false);
            StartTutorial();
        }
        else
        {
            _storyScreens[currentStory].gameObject.SetActive(true);
        }
        
        
        

        
    }

    private void StartTutorial()
    {
        ShowTutorial();
        _hand.localPosition = _handPos_1.localPosition;
        _hand.gameObject.SetActive(true);
        _handTween?.Kill();

        _handTween = DOTween.Sequence()
            .Append(_hand.DOLocalMove(_handPos_2.localPosition, HAND_ANIMATION_DURATION))
            .AppendInterval(RESTART_DELAY)
            .SetLoops(-1, LoopType.Restart);
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
        _storyScreen.onClick.RemoveListener(StartNextStory);
    }
}