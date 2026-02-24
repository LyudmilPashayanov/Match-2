using MergeIt.Core.Messages;
using UnityEngine;
using UnityEngine.UI;

public class EnableTutorialHandMessage  : IMessage
{
    public bool Enabled { get; set; }
    public RectTransform TutorialHand { get; set; }
    public Image TutorialHandImage { get; set; }
}