namespace MergeTwo
{
    public interface IEventIconMerged : IEventBusSubscriber
    {
        void IconMerged(Icon icon);
    }
}