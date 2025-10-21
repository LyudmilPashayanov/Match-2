namespace MergeTwo
{
    public interface IEventIShowInfoPopup : IEventBusSubscriber
    {
        void Show(IconType iconType);
    }
}
