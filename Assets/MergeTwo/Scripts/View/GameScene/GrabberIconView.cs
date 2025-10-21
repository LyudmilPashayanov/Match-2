using System.Collections;
using UnityEngine;

namespace MergeTwo
{
    public class GrabberIconView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _iconRenderer;

        Icon _icon;
        GameObject _currentColide;

        public Icon Icon => _icon;
       // public IconView ColideIcon;

        public void Init(Icon icon)
        {
            _icon = icon;

            if (icon.IconType != IconType.None)
            {
                Sprite sprite = GameContext.GetInstance<Config>().GetSprite(icon.IconType, icon.Value);
                _iconRenderer.sprite = sprite;
            }
            else
            {
                _iconRenderer.sprite = null;
            }
        }

        public float Width => _iconRenderer.size.x;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _currentColide)
                return;

            _currentColide = other.gameObject;
            var iconView = _currentColide.GetComponent<IconView>();
            Grabber.Instance.AddCollidedIcon(iconView);
        }

        void OnTriggerExit(Collider other)
        {
            _currentColide = null;
        }

        private void OnDisable()
        {
            _currentColide = null;
        }
    }
}