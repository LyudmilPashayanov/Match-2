using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class PieceView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] public int ID;
        [SerializeField] private float _duration = 1.0f;

        private EventBus _eventBus;
        private State _state;

        private void Start()
        {
            _eventBus = GameContext.GetInstance<EventBus>();
            _state = GameContext.GetInstance<State>();

            _icon.gameObject.SetActive(Logic.Room.IsPiecePurchased(ID, _state));
        }

        public IEnumerator ShowAnimations() 
        {
            gameObject.SetActive(true);
            float time = 0;
            while (time < _duration) 
            {
                float value = time / _duration;
                SetScale(value);
                SetAlpha(value);
                time += Time.deltaTime;
                yield return null;
            }
            Logic.Room.AnimatePiece(ID, _state);
        }

        private void SetScale(float value)
        {
            _icon.transform.localScale = new Vector3(value, value, 1);
        }

        private void SetAlpha(float value) 
        {
            _icon.color = new Color(_icon.color.r, _icon.color.g, _icon.color.b, value);
        }
      
    }
}
