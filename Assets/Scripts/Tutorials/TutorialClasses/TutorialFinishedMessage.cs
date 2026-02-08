using MergeIt.Core.Messages;

public class TutorialFinishedMessage : IMessage
{ 
    public string TutorialFinishedName { get; set; }
}

public class TutorialInProgressMessage : IMessage
{ 
    public string TutorialCurrentlyInProgressName { get; set; }
}
