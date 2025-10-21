namespace MergeTwo
{
    public interface IEventSetIconOnField : IEventBusSubscriber
    {
        void Click(IconType iconType, int index);
    }

}