using System;
using System.Collections.Generic;

namespace MergeTwo
{
    [Serializable]
    public class State
    {
        public List<IconList> ListField = new();
        public List<int> ClaimedOrders = new();
        public int Stars;
        public int Energy;
        public List<int> PurchasedPieces = new();
        public List<int> AnimatedPieces = new();
        public int CurrentRoom;
        public bool IsRoomCompleted;
        public AnimationItem FlyIconAnimation;
        public List<Icon> IconToTake = new();
    }
}
