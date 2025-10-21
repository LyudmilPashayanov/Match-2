using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MergeTwo
{
    public class OrdersViewController : MonoBehaviour,
        IEventIconMerged,
        IEventOrderClaimed
    {
        [SerializeField] OrderItem[] _items;
        [SerializeField] TextMeshProUGUI sf_starLabel;
        [SerializeField] Grabber _grabber;
        [SerializeField] GameObject _framePrefab;

        Config _config;
        State _state;
        EventBus _eventBus;
        Dictionary<Pos, GameObject> _framesByPos = new();
        Dictionary<int, int> _indexByIds = new();

        private void Start()
        {
            _eventBus = GameContext.GetInstance<EventBus>();
            _eventBus.Subscribe<IEventIconMerged>(this);
            _eventBus.Subscribe<IEventOrderClaimed>(this);
            _config = GameContext.GetInstance<Config>();
            _state = GameContext.GetInstance<State>();
            UpdateOrderView();
        }

        void UpdateOrderView()
        {
            foreach (var frame in _framesByPos.Values)
            {
                frame.SetActive(false);
            }
            List<Order> orders = null;

            if (_indexByIds.Count > 0)
            {
                var displayedOrder = _indexByIds.First();
                orders = Logic.GetNextOrders(_config.Orders, _state, displayedOrder.Key, displayedOrder.Value);
            }
            else 
            {
                orders = Logic.GetNextOrders(_config.Orders, _state);
            }

            for (int i = 0; i < orders.Count; i++)
            {
                Order order = orders[i];
                _items[i].DoneOrder.UpdateOrderView(order);
                _items[i].UndoneOrder.UpdateOrderView(order);
                bool isOrderReady = Logic.IsOrderReady(order, _state);
                _items[i].DoneOrder.gameObject.SetActive(isOrderReady);
                _items[i].UndoneOrder.gameObject.SetActive(!isOrderReady);

                _indexByIds[order.ID] = i;
                AddFrames(order);
            }
            sf_starLabel.text = _state.Stars.ToString();
        }

        private void AddFrames(Order order)
        {
            List<Pos> orderIconPositions = Logic.GetOrderIconPositions(order, _state);
            foreach (var pos in orderIconPositions)
            {
                if (!_framesByPos.ContainsKey(pos))
                {
                    IconView iconView = _grabber.Icons.First(i => i.PosAtMatrix.x == pos.x && i.PosAtMatrix.y == pos.y);
                    _framesByPos[pos] = Instantiate(_framePrefab, iconView.transform);
                    _framesByPos[pos].transform.localPosition = Vector3.zero;
                }
                else
                {
                    _framesByPos[pos].SetActive(true);
                }
            }
        }

        void IEventIconMerged.IconMerged(Icon icon)
        {
            UpdateOrderView();
        }

        void IEventOrderClaimed.OrderClaimed(int id)
        {
            _indexByIds.Remove(id);
            UpdateOrderView();
        }

        private void OnDestroy()
        {
            _eventBus.UnSubscribe<IEventIconMerged>(this);
            _eventBus.UnSubscribe<IEventOrderClaimed>(this);
        }

        [Serializable]
        private class OrderItem
        {
            public OrderView UndoneOrder;
            public OrderView DoneOrder;
        }
    }
}