using UnityEngine;

namespace MergeTwo
{
    public class IconView : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _iconSpriteRenderer;
        [SerializeField] BoxCollider _colider;

        Icon _icon;
        SpriteRenderer _light;

        public Icon Icon => _icon;
        public Pos PosAtMatrix;

        public void Init(Icon icon)
        {
            _icon = icon;

            if (icon.IconType != IconType.None)
            {
                Sprite sprite = GameContext.GetInstance<Config>().GetSprite(icon.IconType, icon.Value);
                _iconSpriteRenderer.sprite = sprite;
                _colider.enabled = true;

                if (Logic.IsSpawner(icon.IconType))
                {
                    Config config = GameContext.GetInstance<Config>();
                    _light = Instantiate(config.LightPrefab).GetComponent<SpriteRenderer>();
                    _light.transform.SetParent(transform);
                    _light.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                _iconSpriteRenderer.sprite = null;
                _colider.enabled = false;
                if (_light != null)
                    Destroy(_light.gameObject);
            }

            gameObject.name = icon.ToString();
        }

        public bool IsInside(Vector3 pos)
        {
            pos = new Vector3(pos.x, pos.y, transform.position.z);
            return _colider.bounds.Contains(pos);
        }

        public void SetVisible(bool value)
        {
            _iconSpriteRenderer.enabled = value;
            if (_light != null)
                _light.enabled = value;
        }

        public void SetColor(Color color)
        {
            _iconSpriteRenderer.color = color;
            if (_light != null)
                _light.color = color;
        }

        private void OnValidate()
        {
            if (_iconSpriteRenderer == null)
                _iconSpriteRenderer = GetComponent<SpriteRenderer>();

            if (_colider == null)
                _colider = GetComponent<BoxCollider>();
        }
    }
}
