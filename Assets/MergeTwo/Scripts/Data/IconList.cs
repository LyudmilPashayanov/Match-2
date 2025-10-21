using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MergeTwo
{
    [Serializable]
    public class IconList
    {
#if UNITY_EDITOR
        [HideInInspector] public string Name;
#endif
        public List<Icon> Icons = new();

        public override string ToString()
        {
            if (Icons != null && Icons.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var icon in Icons)
                {
                    sb.Append($"{icon} ");
                }
                return sb.ToString();
            }
            return base.ToString();
        }
    }
}