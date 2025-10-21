using UnityEngine;
using UnityEngine.UI;

namespace MergeTwo
{
    public class ProgressBar : MonoBehaviour
    {
        [Range(0, 1.0f)][SerializeField] private float m_value;
        [SerializeField] private Image _mask;
        [SerializeField] private RectTransform _target;
        [SerializeField] private float max;
        [SerializeField] private Vector3 _startPos;

        

        public void SetValue(float value)
        {
            Rect rect = _mask.rectTransform.rect;
            Vector3 localPos = _target.localPosition;
            _target.localPosition = new Vector3(value * rect.width, 0, 0) + _startPos;
        }

        [ContextMenu("SetStartPos")]
        public void SetStartPos() 
        {
            _target.localPosition = new Vector3(-_mask.rectTransform.rect.width, _target.localPosition.y, _target.localPosition.z);           
            _startPos = _target.localPosition;
        }

        private void OnValidate()
        {
            //if (_mask != null && _target != null) { 
            //    SetValue(m_value);
            //}
        }
    }
}
