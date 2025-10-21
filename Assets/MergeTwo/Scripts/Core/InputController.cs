using System;
using UnityEngine;

namespace MergeTwo
{
    public class InputController : MonoBehaviour
    {
        public bool IsTap;
        public bool IsTapDown;
        public Vector3 CursorPos;
        public bool IsDoubleClick;

        private float _doubleClickTime = 0.25f;
        private float _lastClickTime = 0f;

        public Action OnDoubleClick;

        void Update()
        {
            IsTap = Input.GetMouseButton(0);
            IsTapDown = Input.GetMouseButtonDown(0);

            CursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0)) 
            {
                float timeSinceLastClick = Time.time - _lastClickTime;

                if (timeSinceLastClick <= _doubleClickTime)
                {
                    OnDoubleClick?.Invoke();
                }

                _lastClickTime = Time.time;
            }
        }
    }
}