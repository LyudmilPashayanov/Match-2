using System;
using UnityEngine;

namespace MergeTwo
{
    public class TapController : MonoBehaviour
    {
        public Action OnClick;

        private void OnMouseDown()
        {
           OnClick?.Invoke();
        }
    }
}
