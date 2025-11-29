using System;
using System.Collections.Generic;
using System.Linq;
using MergeIt.Core.Configs.Elements;
using MergeIt.Core.FieldElements;
using MergeIt.Core.Messages;
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
        private int _amountOfSimultatiousOrders = 2;

        private List<OrderView> _spawnedOrders = new List<OrderView>();
        private List<int> _completedOrders;
        private Dictionary<ElementConfig, int> _typeAmounts = new Dictionary<ElementConfig, int>();

        private void Start()
        {
            _messageBus = DiContainer.Get<IMessageBus>();

            _messageBus.AddListener<MergeElementsMessage>(OnItemMerged);
            _messageBus.AddListener<SplitElementMessage>(OnItemSplit);
            _messageBus.AddListener<GeneratorOpenedMessage>(OnItemGenerated);
            _messageBus.AddListener<StartGameMessage>(OnGameStarted);

            _fieldLogicModel = DiContainer.Get<FieldLogicModel>();
            _fieldElementVisualFactory = DiContainer.Get<IFieldElementVisualFactory>();

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
                if (_spawnedOrders.Count >= _amountOfSimultatiousOrders)
                {
                    return;
                }

                if (_completedOrders.Contains(orderDefinition.OrderId))
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
        }

        private void OnOrderCompleted(OrderView order)
        {
            // Remove from the orders view
            _spawnedOrders.Remove(order);
            Destroy(order.gameObject);
            // save that it was completed
            _completedOrders.Add(order.OrderDefinition.OrderId);
            //save in player prefs
            SaveCompletedOrders();
            // add new order
            InitializeOrders();
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
        
        private void OnItemGenerated(GeneratorOpenedMessage _ = null)
        {
            Debug.Log("OnItemGenerated");
            UpdateFieldData();
            UpdateOrdersView();
        }
        
        private void OnGameStarted(StartGameMessage obj)
        {
            Debug.Log("OnGameStarted");
            UpdateFieldData();
            UpdateOrdersView();
        }
        
        public void Dispose()
        {
            _messageBus.RemoveListener<MergeElementsMessage>(OnItemMerged);
            _messageBus.RemoveListener<SplitElementMessage>(OnItemSplit);
        }
    }
}
