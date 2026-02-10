using DG.Tweening;
using MergeIt.Core.Messages;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class SecondUnblockTutorial : Tutorial
{
    private const float HAND_ANIMATION_DURATION = 1f;
    private const float RESTART_DELAY = 0.5f;
    
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

        CheckToStart();
    }

    private void CheckToStart()
    {
        if (PlayerPrefs.HasKey(FirstTutorialName))
        {
            if (PlayerPrefs.GetInt(FirstTutorialName) == 1)
            {
                StartTutorial();
            }
        }
    }
    
    private void OnFirstTutorialFinished(TutorialFinishedMessage message)
    {
        if (message.TutorialFinished.TutorialName == FirstTutorialName)
        {
            StartTutorial();
        }
    }

    private void StartTutorial()
    {
        ShowTutorial();
                
        DisableHints();

        _hand.localPosition = _handPos_1.localPosition;
        _hand.gameObject.SetActive(true);
        _handTween?.Kill();

        _handTween = DOTween.Sequence()
            .Append(_hand.DOLocalMove(_handPos_2.localPosition, HAND_ANIMATION_DURATION))
            .AppendInterval(RESTART_DELAY)
            .SetLoops(-1, LoopType.Restart);
    }


    private void DisableHints()
    {
        EnableTutorialHandMessage enableTutorialHandMessage = new EnableTutorialHandMessage(){Enabled = false};
        _messageBus.Fire(enableTutorialHandMessage);
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

    private void OnDisable()
    {
        _messageBus.RemoveListener<TutorialFinishedMessage>(OnFirstTutorialFinished);
        _messageBus.RemoveListener<MergeElementsMessage>(OnMergeElementMessageHandler);
    }
}
