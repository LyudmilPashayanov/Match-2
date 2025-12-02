using System;
using System.Collections.Generic;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.FieldElements;
using MergeIt.Core.Messages;
using MergeIt.Core.Services;
using MergeIt.Game.Factories.FieldElement;
using MergeIt.Game.Messages;
using MergeIt.SimpleDI;
using UnityEngine;

[Serializable]
public class IntListWrapper
{
    public List<int> Values = new List<int>();
}

namespace MergeIt.Game.Field
{
    public class OrdersManager : MonoBehaviour, IDisposable
    {
        private const string COMPLETED_ORDERS_KEY = "completedOrdersList"; 
        private const int OFFSET = 20;

        [SerializeField] private RectTransform _scrollViewContent;
        [SerializeField] private OrderView _orderPrefab;
        
        [SerializeField] private OrderList _orderList;

        private FieldLogicModel _fieldLogicModel;
        private IFieldElementVisualFactory _fieldElementVisualFactory;
        private IMessageBus _messageBus;

        private float _orderPrefabWidth;
        private int _amountOfSimultaniousOrders = 2;

        private List<OrderView> _spawnedOrders = new List<OrderView>();
        private List<int> _completedOrders;
        private Dictionary<ElementConfig, int> _typeAmounts = new Dictionary<ElementConfig, int>();
        private IGameFieldService _fieldService;

        private void Start()
        { 
            PlayerPrefs.DeleteKey(COMPLETED_ORDERS_KEY);
            
            _fieldLogicModel = DiContainer.Get<FieldLogicModel>();
            _fieldElementVisualFactory = DiContainer.Get<IFieldElementVisualFactory>();
            _fieldService = DiContainer.Get<IGameFieldService>();

            _messageBus = DiContainer.Get<IMessageBus>();

            _messageBus.AddListener<MergeElementsMessage>(OnItemMerged);
            _messageBus.AddListener<SplitElementMessage>(OnItemSplit);
            _messageBus.AddListener<ClickElementMessage>(OnItemClicked);
            _messageBus.AddListener<LoadedGameMessage>(OnGameLoaded);
            _messageBus.AddListener<RemoveFromInventoryMessage>(RemoveFromInventory);
            _messageBus.AddListener<AddToInventoryMessage>(MoveToInventory);

            
            _orderPrefabWidth = _orderPrefab.GetComponent<RectTransform>().rect.width + OFFSET;

            _completedOrders = LoadCompletedOrders();
            
            InitializeOrders();
            UpdateFieldData();
            UpdateOrdersView();
        }

        private void InitializeOrders()
        {
            foreach (var orderDefinition in _orderList.OrderDefinitions)
            {
                if (_spawnedOrders.Count >= _amountOfSimultaniousOrders)
                {
                    return;
                }

                if (_completedOrders.Contains(orderDefinition.OrderId))
                {
                    continue;
                }

                bool currentlySpawned = false;
                foreach (var spawnedOrder in _spawnedOrders)
                {
                    if (spawnedOrder.OrderDefinition.OrderId == orderDefinition.OrderId)
                    {
                        currentlySpawned = true;
                    }
                }

                if (currentlySpawned)
                {
                    continue;
                }
                
                AddOrder(orderDefinition);
            }
        }

        private void AddOrder(OrderDefinition orderDefinition)
        {
            OrderView newOrder = Instantiate(_orderPrefab, _scrollViewContent);

            newOrder.Init(OnOrderCompleted);
            newOrder.Setup(orderDefinition);

            _spawnedOrders.Add(newOrder);

            UpdateScrollViewContent();
            UpdateFieldData();
            UpdateOrdersView();
        }

        private void OnOrderCompleted(OrderView order)
        {
            // Remove from the orders view
            _spawnedOrders.Remove(order);
            Destroy(order.gameObject);
            // Remove items from the field
            RemoveItemsFromFields(order.OrderDefinition);
            //Spawn experience on the board.

            GenerateRewardOnField(order);

            // save that it was completed
            _completedOrders.Add(order.OrderDefinition.OrderId);
            //save in player prefs
            SaveCompletedOrders();
            // add new order
            InitializeOrders();
        }

        private void GenerateRewardOnField(OrderView order)
        {
            GridPoint? pointContainer = _fieldService.GetFreeCell();

            if (pointContainer != null)
            {
                GridPoint point = pointContainer.Value;
                IFieldElement newElement = _fieldService.CreateNewElement(order.OrderDefinition.reward, point);

                var message = new CreateElementMessage
                {
                    FromPosition = order.transform.position,
                    ToPoint = point,
                    NewElement = newElement
                };

                _messageBus.Fire(message);
            }
        }

        private void UpdateScrollViewContent()
        {
            // setting new scroll view content width
            float newContentWidth = _spawnedOrders.Count * (_orderPrefabWidth + OFFSET);
            _scrollViewContent.sizeDelta.Set(newContentWidth, _scrollViewContent.sizeDelta.y);

            int counter = 0;
            foreach (var order in _spawnedOrders)
            {
                // setting new order position in content scroll view
                order.transform.localPosition = new Vector3((counter * _orderPrefabWidth) + OFFSET, 0, 0);
                counter++;
            }
        }

        private void UpdateOrdersView()
        {
            foreach (var order in _spawnedOrders)
            {
                order.UpdateState(_typeAmounts);
            }
        }

        private void UpdateFieldData()
        {
            int fieldWidth = _fieldLogicModel.FieldWidth;
            int fieldHeight = _fieldLogicModel.FieldHeight;
            _typeAmounts.Clear();
            
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    var point = GridPoint.Create(i, j);
                    if (_fieldLogicModel.FieldElements.TryGetValue(point, out var fieldElement))
                    {
                        var keyType = fieldElement.ConfigParameters.ElementConfig;
                        if (_typeAmounts.ContainsKey(keyType))
                        {
                            _typeAmounts[keyType]++;
                        }
                        else
                        {
                            _typeAmounts.Add(keyType, 1);
                        }
                    }
                }
            }
        }

        private void RemoveItemsFromFields(OrderDefinition orderDefinition)
        {
            int fieldWidth = _fieldLogicModel.FieldWidth;
            int fieldHeight = _fieldLogicModel.FieldHeight;
            foreach (var order in orderDefinition.RequiredItems)
            {
                int removedCounter = 0;
                for (int i = 0; i < fieldHeight; i++)
                {
                    for (int j = 0; j < fieldWidth; j++)
                    {
                        var point = GridPoint.Create(i, j);
                        if (_fieldLogicModel.FieldElements.TryGetValue(point, out var fieldElement))
                        {
                            if (order.Type == fieldElement.ConfigParameters.ElementConfig)
                            {
                                var remove = new RemoveElementMessage
                                {
                                    RemoveAtPoint = point
                                };
                                _messageBus.Fire(remove);
                                removedCounter++;
                            }
                        }
                    }
                    if (removedCounter == order.Amount)
                    {
                        break;
                    }
                }
            }
        }
        
        
        private List<int> LoadCompletedOrders()
        {
            if (PlayerPrefs.HasKey(COMPLETED_ORDERS_KEY) == false)
            {
                return new List<int>();
            }

            string json = PlayerPrefs.GetString(COMPLETED_ORDERS_KEY);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<int>();
            }

            var wrapper = JsonUtility.FromJson<IntListWrapper>(json);
            return wrapper?.Values ?? new List<int>();
        }

        private void SaveCompletedOrders()
        {
            var wrapper = new IntListWrapper
            {
                Values = _completedOrders
            };

            string json = JsonUtility.ToJson(wrapper);

            PlayerPrefs.SetString(COMPLETED_ORDERS_KEY, json);
            PlayerPrefs.Save();
        }
        
        private void OnItemMerged(MergeElementsMessage _ = null)
        {
            Debug.Log("OnItemMerged");
            UpdateFieldData();
            UpdateOrdersView();
        }
        
        private void OnItemSplit(SplitElementMessage _ = null)
        {
            Debug.Log("OnItemSplit");
            UpdateFieldData();
            UpdateOrdersView();
        }
        
        private void OnItemClicked(ClickElementMessage _ = null)
        {
            Debug.Log("OnItemGenerated");
            UpdateFieldData();
            UpdateOrdersView();
        }
        
        private void OnGameLoaded(LoadedGameMessage obj)
        {
            UpdateFieldData();
            UpdateOrdersView();
        }
        
        private void MoveToInventory(AddToInventoryMessage obj)
        {
            Debug.Log("MoveToInventory");
            UpdateFieldData();
            UpdateOrdersView();
        }

        private void RemoveFromInventory(RemoveFromInventoryMessage obj)
        {
            Debug.Log("RemoveFromInventory");
            UpdateFieldData();
            UpdateOrdersView();
        }
        
        public void Dispose()
        {
            if (_messageBus != null)
            {
                _messageBus.RemoveListener<MergeElementsMessage>(OnItemMerged);
                _messageBus.RemoveListener<SplitElementMessage>(OnItemSplit);
                _messageBus.RemoveListener<ClickElementMessage>(OnItemClicked);
                _messageBus.RemoveListener<LoadedGameMessage>(OnGameLoaded);
                _messageBus.RemoveListener<AddToInventoryMessage>(MoveToInventory);  
                _messageBus.RemoveListener<RemoveFromInventoryMessage>(RemoveFromInventory);
            }

        }
    }
}
