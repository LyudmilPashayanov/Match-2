using UnityEngine;

namespace MergeTwo
{
    public interface IEventConfigButtonClick : IEventBusSubscriber
    {
        void Click(ConfigButtonViewField button, Vector2Int pos);
    } 
}
