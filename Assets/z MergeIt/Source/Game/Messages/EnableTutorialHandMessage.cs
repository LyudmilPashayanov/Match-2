using MergeIt.Core.Messages;
using UnityEngine;

public class EnableTutorialHandMessage  : IMessage
{
    public bool Enabled { get; set; }
    public RectTransform TutorialHand { get; set; }
}