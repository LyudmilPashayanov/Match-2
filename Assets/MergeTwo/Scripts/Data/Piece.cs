using System;
using System.Collections.Generic;
using UnityEngine;

namespace MergeTwo
{
    [Serializable]
    public class Piece
    {
        public string Name;
        public int ID;
        public int Price = 1;
        public Sprite Icon;
        public string Text;
        public List<int> Conditions = new();
    }
}
