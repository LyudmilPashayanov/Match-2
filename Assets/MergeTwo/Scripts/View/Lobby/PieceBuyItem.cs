using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class PieceBuyItem : MonoBehaviour
    {
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _starLabel;
        [SerializeField] TextMeshProUGUI _textLabel;
        [SerializeField] Transform _star;

        int _id;
        State _state;
        Config _config;
        EventBus _eventBus;

        private void Start()
        {
            _state = GameContext.GetInstance<State>();
            _config = GameContext.GetInstance<Config>();
            _eventBus = GameContext.GetInstance<EventBus>();
        }

        public void Display(Piece piece)
        {
            _icon.sprite = piece.Icon;
            _starLabel.text = piece.Price.ToString();
            _textLabel.text = piece.Text;
            _id = piece.ID;
        }

        public void Purchase()
        {
            
            if (Logic.Room.IsCanPurchasePiece(_id, _state, _config.Rooms))
            {
                Logic.Room.BuyPiece(_id, _state, _config.Rooms);
                _eventBus.Emmit<IEventPiecePurchased>(e => e.PiecePurchased(_id, _star));
            }
        }
    }
}
