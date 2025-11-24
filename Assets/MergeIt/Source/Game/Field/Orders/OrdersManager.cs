using System;
using MergeIt.Core.FieldElements;
using MergeIt.Core.Messages;
using MergeIt.Game.Factories.FieldElement;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

namespace MergeIt.Game.Field
{
    public class OrdersManager : MonoBehaviour, IDisposable
    {
        [SerializeField] private OrderList _orderList;
        [SerializeField] private OrdersView _ordersView;
        
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
                        // tODO: go through all items and see if they suit an order from the order list.
                        // todo: if yes, light that order in green. so it can be given in.
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
