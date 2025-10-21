using System;
using UnityEngine;

namespace MergeTwo
{
    [Serializable]
    public class Icon
    {
#if UNITY_EDITOR
        [HideInInspector]public string Name;
#endif

        public int Value;
        public IconType IconType; 
        public Pos Pos;
        public int Capacity;
        public long RechargeTime;

        public Icon Clone() 
        {
            return new Icon { 
                Value = Value,
                IconType = IconType,
#if UNITY_EDITOR
                Name = Name,
#endif
                Pos = Pos,
                Capacity = Capacity
            };
        }

        public override string ToString()
        {
            return $"{IconType}{Pos}";
        }
    } 
}
