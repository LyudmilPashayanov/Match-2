using System;
using UnityEngine;

namespace MergeTwo
{
    [Serializable]
    public class IconOrder 
    {
        [HideInInspector] public string Name;
        public Icon Icon;
        public int Amount;

        public override string ToString()
        {
            return $"{5} {Icon}";
        }
    }
}