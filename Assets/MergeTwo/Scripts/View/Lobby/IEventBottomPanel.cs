namespace MergeTwo
{
    public interface IEventBottomPanel : IEventBusSubscriber
    {
        void OnHomeClick();
        void OnAreaClick();
    }
}
