using MergeIt.Core.Messages;
using MergeIt.Game;

public class OrderAvailableToServeMessage : IMessage
{ 
    public OrderView AvailableToServeOrder { get; set; }
}

public class OrderCompletedMessage : IMessage
{
    public OrderView CompletedOrder { get; set; }
}