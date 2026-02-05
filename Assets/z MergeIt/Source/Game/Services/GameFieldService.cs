// Copyright (c) 2024, Awessets

using System;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.FieldElements;
using MergeIt.Core.Helpers;
using MergeIt.Core.Messages;
using MergeIt.Core.Services;
using MergeIt.Game.Factories.Field;
using MergeIt.Game.Factories.FieldElement;
using MergeIt.Game.Field;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using MergeIt.SimpleDI.ReservedInterfaces;

namespace MergeIt.Game.Services
{
    public class GameFieldService : IGameFieldService, IInitializable, IDisposable
    {
        [Introduce]
        private IConfigsService _configsService;

        [Introduce]
        private IFieldElementFactory _fieldElementFactory;

        [Introduce]
        private IFieldFactory _fieldFactory;

        [Introduce]
        private FieldLogicModel _fieldLogicModel;

        [Introduce]
        private GameServiceModel _gameServiceModel;

        [Introduce]
        private IMessageBus _messageBus;

        public void Dispose()
        {
            _messageBus.RemoveListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
        }

        public GridPoint? GetFreeCell()
        {
            int fieldHeight = _fieldLogicModel.FieldHeight;
            int fieldWidth = _fieldLogicModel.FieldWidth;

            var randomHeight = ListExtensions.GenerateShuffledArray(fieldHeight);
            var randomWidth = ListExtensions.GenerateShuffledArray(fieldWidth);

            for (int i = 0; i < randomHeight.Count; i++)
            {
                for (int j = 0; j < randomWidth.Count; j++)
                {
                    int row = randomHeight[i];
                    int column = randomWidth[j];

                    var point = GridPoint.Create(row, column);
                    if (!_fieldLogicModel.FieldElements.ContainsKey(point))
                    {
                        return point;
                    }
                }
            }

            return null;
        }

        public IFieldElement CreateNewElement(ElementConfig config, GridPoint point, bool blocked = false)
        {
            IFieldElement newElement =
                _fieldElementFactory.CreateFieldElement(config, point, blocked);

            return newElement;
        }

        public void Initialize()
        {
            _messageBus.AddListener<LoadedGameMessage>(OnLoadedGameMessageHandler);
        }

        private void OnLoadedGameMessageHandler(LoadedGameMessage message)
        {
            FieldPresenter field = _fieldFactory.CreateField(_gameServiceModel.MainCanvas.transform);
            field.Initialize();
        }
    }
}