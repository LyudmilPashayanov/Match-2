using System.Collections.Generic;
using UnityEngine;

namespace MergeIt.Game
{
    public class OrdersView : MonoBehaviour
    {
        private const int OFFSET = 20;
        
        [SerializeField] private RectTransform _scrollViewContent;
        [SerializeField] private OrderView _orderPrefab;

        private List<OrderView> _spawnedOrders = new List<OrderView>();
        private float _orderPrefabWidth;

        private void Start()
        {
            _orderPrefabWidth = _orderPrefab.GetComponent<RectTransform>().rect.width + OFFSET;        
        }

        public void AddOrder(OrderDefinition orderDefinition)
        {
            OrderView newOrder = Instantiate(_orderPrefab, _scrollViewContent);
            
            newOrder.Setup(orderDefinition);
            
            _spawnedOrders.Add(newOrder);

            UpdateScrollViewContent();
        }

        public void SetOrderActive()
        {
            
        }

        private void SetOrderInactive()
        {
            
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
        
        public void RemoveOrder()
        {
            
        }
    }
}
