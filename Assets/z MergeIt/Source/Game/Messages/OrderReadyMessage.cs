using MergeIt.Core.Messages;

namespace MergeIt.Game.Messages
{
    public class OrderReadyMessage : IMessage
    {
        public int AvailableOrders { get; set; }
    }
}