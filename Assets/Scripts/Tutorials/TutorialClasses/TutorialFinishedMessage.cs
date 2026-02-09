using MergeIt.Core.Messages;

public class TutorialFinishedMessage : IMessage
{ 
    public Tutorial TutorialFinished { get; set; }
}

public class TutorialInProgressMessage : IMessage
{ 
    public string TutorialCurrentlyInProgressName { get; set; }
}
