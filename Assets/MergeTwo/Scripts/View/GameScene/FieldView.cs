using System.Collections.Generic;
using UnityEngine;

namespace MergeTwo
{
    public class FieldView : MonoBehaviour,
        IEventFieldUpdate,
        IEventOrderClaimed
    {
        public List<IconView> IconViews;

        Dictionary<Pos, IconView> _viewsByPos = new();
        State _state;

        private void Start()
        {
            var eventBus = GameContext.GetInstance<EventBus>();
            eventBus.Subscribe<IEventFieldUpdate>(this);
            eventBus.Subscribe<IEventOrderClaimed>(this);

            _state = GameContext.GetInstance<State>();
            ChacheViews();
            UpdateView();
        }

        void ChacheViews()
        {
            foreach (var icon in IconViews)
            {
                _viewsByPos.Add(icon.PosAtMatrix, icon);
            }
        }

        public void UpdateView()
        {
            for (int i = 0; i < _state.ListField.Count; i++)
            {
                for (int j = 0; j < _state.ListField[i].Icons.Count; j++)
                {
                    Icon icon = _state.ListField[i].Icons[j];
                    _viewsByPos[icon.Pos].Init(icon);
                }
            }
        }

        void IEventOrderClaimed.OrderClaimed(int _)
        {
            UpdateView();
        }

        private void OnDestroy()
        {
            var eventBus = GameContext.GetInstance<EventBus>();
            eventBus.UnSubscribe<IEventFieldUpdate>(this);
            eventBus.UnSubscribe<IEventOrderClaimed>(this);
        }
    } 
}
