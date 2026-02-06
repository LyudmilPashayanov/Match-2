using System;
using MergeIt.Core.Messages;
using MergeIt.Game.Field;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

public class FirstMergeTutorial : Tutorial
{
    private IMessageBus _messageBus;
    private FieldLogicModel _fieldLogicModel;

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
                ShowTutorial();
                break;
            }
        }
    }
   
    private void OnMergeElementMessageHandler(MergeElementsMessage message)
    {
        if (message.NewElement.InfoParameters.Name == "Pair of Gloves")
        {
            HideTutorial();
        }
    }

    private void OnDisable()
    {
        _messageBus?.RemoveListener<LoadedGameMessage>(OnGameLoadedMessageHandler);
        _messageBus?.RemoveListener<MergeElementsMessage>(OnMergeElementMessageHandler);
    }
}