using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class OrderView : MonoBehaviour
    {
        [SerializeField] IconView[] _views;
        [SerializeField] Button sf_button;

        State _state;
        Config _config;
        Order pf_order;
        EventBus pf_eventBus;

        public int OrderID => pf_order != null ? pf_order.ID : -1;

        private void Start()
        {
            _state = GameContext.GetInstance<State>();
            _config = GameContext.GetInstance<Config>();
            sf_button?.onClick.AddListener(OnClick);
            pf_eventBus = GameContext.GetInstance<EventBus>();

        }

        private void OnClick()
        {
            Logic.ClaimOrder(pf_order, _state);
            pf_eventBus.Emmit<IEventOrderClaimed>(e => e.OrderClaimed(pf_order.ID));
        }

        public void UpdateOrderView(Order order)
        {
            pf_order = order;
            _state ??= GameContext.GetInstance<State>();
            _config ??= GameContext.GetInstance<Config>();

            for (int i = 0; i < order.IconsToCollect.Count; i++)
            {
                IconOrder iconOrder = order.IconsToCollect[i];
                UpdateView(iconOrder, _views[i]);
            }
        }

        void UpdateView(IconOrder iconOrder, IconView view)
        {
            int iconAmount = Logic.CalculateAmount(iconOrder.Icon, _state);
            view.Label.text = $"{iconAmount}/{iconOrder.Amount}";
            Sprite sp = _config.GetSprite(iconOrder.Icon.IconType, iconOrder.Icon.Value);
            view.IconImage.sprite = sp;
            view.IconImage.SetNativeSize();
            view.IconImage.rectTransform.sizeDelta = view.IconImage.rectTransform.sizeDelta * 0.5f;
            view.InfoButton?.onClick.RemoveAllListeners();
            view.InfoButton?.onClick.AddListener(() => pf_eventBus.Emmit<IEventIShowInfoPopup>(e => e.Show(iconOrder.Icon.IconType)));
        }

        [Serializable]
        private class IconView
        {
            public Image IconImage;
            public TextMeshProUGUI Label;
            public Button InfoButton;
        }
    }
}