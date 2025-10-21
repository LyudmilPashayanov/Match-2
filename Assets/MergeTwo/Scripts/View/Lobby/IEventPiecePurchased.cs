using UnityEngine;

namespace MergeTwo
{
    public interface IEventPiecePurchased : IEventBusSubscriber
    {
        void PiecePurchased(int id, Transform star);
    }
}
