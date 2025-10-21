using System.Collections;
using System.Linq;
using UnityEngine;

namespace MergeTwo
{
    public class FlyIconAnimator : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _flyIcon;
        [SerializeField] IconView[] _icons;
        [SerializeField] float _duration = 0.75f;

        private Config _config;
        private State _state;

        public bool IsAnimated;

        private void Start()
        {
            _config = GameContext.GetInstance<Config>();
            _state = GameContext.GetInstance<State>();
            _flyIcon.gameObject.SetActive(false);

            if (_state == null)
                Debug.LogError("STATE IS NULL");

            if (_state.ListField == null)
                Debug.LogError("LIST_FIELD IS NULL");

            if (_config == null)
                Debug.LogError("CONFIG IS NULL");
        }

        public void StartAnimation(Pos startPos, Pos endPos, IconType iconType, int iconValue)
        {
            StartCoroutine(FlyAnimation(startPos, endPos, iconType, iconValue));
        }

        private IEnumerator FlyAnimation(Pos startPos, Pos endPos, IconType iconType, int iconValue)
        {
            IsAnimated = true;

            Sprite sprite = _config.GetSprite(iconType, iconValue);
            _flyIcon.sprite = sprite;
            _flyIcon.gameObject.SetActive(true);
            Vector3 start = _icons.First(i => i.PosAtMatrix.x == startPos.x && i.PosAtMatrix.y == startPos.y).transform.position;
            start = new Vector3(start.x, start.y, -1);
            IconView endIconView = _icons.First(i => i.PosAtMatrix.x == endPos.x && i.PosAtMatrix.y == endPos.y);
            Vector3 end = endIconView.transform.position;
            end = new Vector3(end.x, end.y, -1);

            float time = 0;
            while (time < _duration)
            {
                time += Time.deltaTime;
                float value = time / _duration;
                _flyIcon.transform.position = Vector3.Lerp(start, end, value);
                yield return null;
            }
            Icon icon = _state.ListField[endPos.x].Icons[endPos.y];
            endIconView.Init(icon);
            _flyIcon.gameObject.SetActive(false);

            IsAnimated = false;
        }

        public void StartAnimation(Vector3 startPos, Pos endPos, IconType iconType, int iconValue)
        {
            StartCoroutine(FlyAnimation(startPos, endPos, iconType, iconValue));
        }

        private IEnumerator FlyAnimation(Vector3 startPos, Pos endPos, IconType iconType, int iconValue)
        {
            IsAnimated = true;

            Sprite sprite = _config.GetSprite(iconType, iconValue);
            _flyIcon.sprite = sprite;
            _flyIcon.gameObject.SetActive(true);
            Vector3 start = new Vector3(startPos.x, startPos.y, -1);
            IconView endIconView = _icons.First(i => i.PosAtMatrix.x == endPos.x && i.PosAtMatrix.y == endPos.y);
            Vector3 end = endIconView.transform.position;
            end = new Vector3(end.x, end.y, -1);

            float time = 0;
            while (time < _duration)
            {
                time += Time.deltaTime;
                float value = time / _duration;
                _flyIcon.transform.position = Vector3.Lerp(start, end, value);
                yield return null;
            }
            Icon icon = _state.ListField[endPos.x].Icons[endPos.y];
            endIconView.Init(icon);
            _flyIcon.gameObject.SetActive(false);

            IsAnimated = false;
        }
    }
}
