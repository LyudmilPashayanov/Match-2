using System;
using System.Collections.Generic;
using UnityEngine;

namespace MergeTwo
{
    [Serializable]
    public class ConfigRoom
    {
        public string Name;
        public List<Piece> Pieces;
        public Sprite LockedSprite;
        public Sprite CollectedSprite;
        public Sprite WorkInProgressSprite;
        public Icon[] RewardIcons;
    }
}
