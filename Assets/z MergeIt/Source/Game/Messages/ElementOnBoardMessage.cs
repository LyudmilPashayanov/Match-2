using MergeIt.Core.FieldElements;
using MergeIt.Core.Messages;
using UnityEngine;

namespace MergeIt.Game
{
    public class ElementOnBoardMessage : IMessage
    {
        public IFieldElement NewElement { get; set; }
        public Vector3? FromPosition { get; set; }
        public GridPoint ToPoint { get; set; }
    }
}
