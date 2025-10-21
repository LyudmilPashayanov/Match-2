using System;

namespace MergeTwo
{
    public interface IEventBus
    {
        void Emmit<T>(Action<T> subscribers) where T : class, IEventBusSubscriber;
        void Subscribe<S>(S subscriber, int priority = 0) where S : IEventBusSubscriber;
        void UnSubscribe<S>(S suscriber) where S : IEventBusSubscriber;
    } 
}
