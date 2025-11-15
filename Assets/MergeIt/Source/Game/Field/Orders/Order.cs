using System;
using System.Collections.Generic;
using MergeIt.Core.Configs.Elements;
using UnityEngine;

namespace MergeIt.Game
{
    [CreateAssetMenu(fileName = "Order", menuName = "Game/Order")]
    public class Order : ScriptableObject
    {
        public ElementConfig Type;
        public int Amount;
    }
}
