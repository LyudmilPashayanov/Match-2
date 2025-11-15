using System;
using System.Collections.Generic;
using MergeIt.Core.FieldElements;
using MergeIt.Core.Messages;
using MergeIt.Core.MVP;
using MergeIt.Core.Services;
using MergeIt.Game.Effects;
using MergeIt.Game.Effects.Controllers;
using MergeIt.Game.Effects.Parameters;
using MergeIt.Game.Factories.Field;
using MergeIt.Game.Factories.FieldElement;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

namespace MergeIt.Game.Field
{
    public class OrdersManager : MonoBehaviour, IDisposable
    {
        [SerializeField] private OrderList _orderList;

        private FieldLogicModel _fieldLogicModel;
        private IFieldElementVisualFactory _fieldElementVisualFactory;
        private IMessageBus _messageBus; 
     
        private void Start()
        {
            _messageBus.AddListener<MergeElementsMessage>(OnItemMerged);
            
            _fieldLogicModel = DiContainer.Get<FieldLogicModel>();
            _fieldElementVisualFactory = DiContainer.Get<IFieldElementVisualFactory>();
            _messageBus = DiContainer.Get<IMessageBus>();
            PlayerPrefs.GetInt("achievedOrders", 0);
        }

        private void OnItemMerged(MergeElementsMessage obj)
        {
            int fieldWidth = _fieldLogicModel.FieldWidth;
            int fieldHeight = _fieldLogicModel.FieldHeight;
            
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    var point = GridPoint.Create(i, j);
                    if (_fieldLogicModel.FieldElements.TryGetValue(point, out var fieldElement))
                    {
                        var fieldElementPresenter = _fieldElementVisualFactory.CreateFieldElement(fieldElement);
                    }
                }
            }
        }

        public void Dispose()
        {
            _messageBus.RemoveListener<MergeElementsMessage>(OnItemMerged);
        }
    }
}
