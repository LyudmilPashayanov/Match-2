using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MergeTwo
{
    public class EventBus : IEventBus
    {
        private Dictionary<RuntimeTypeHandle, List<PrioritySubscriber>> SubscribersByType = new Dictionary<RuntimeTypeHandle, List<PrioritySubscriber>>();

        public void Emmit<T>(Action<T> action) where T : class, IEventBusSubscriber
        {
            Type type = typeof(T);
            RuntimeTypeHandle handle = type.TypeHandle;
            if (SubscribersByType.ContainsKey(handle))
            {
                List<PrioritySubscriber> eventsSubs = SubscribersByType[handle];
                foreach (PrioritySubscriber prioritySub in eventsSubs)
                {
                    action.Invoke(prioritySub.Subscriber as T);
                }
            }
        }

        public void Subscribe<S>(S subscriber, int prioity = 0) where S : IEventBusSubscriber
        {
            Type type = typeof(S);
            RuntimeTypeHandle handle = type.TypeHandle;
            if (SubscribersByType.ContainsKey(handle))
            {
                //if (SubscribersByType[handle].Contains(subscriber))
                if (SubscribersByType[handle].Any(s => s.Subscriber.GetHashCode() == subscriber.GetHashCode()))
                {
                    Debug.LogError($"Object of type->{type.Name} try to subscribe twice");
                    return;
                }

                SubscribersByType[handle].Add(new PrioritySubscriber
                {
                    Subscriber = subscriber,
                    Priority = prioity
                });

                SubscribersByType[handle].Sort((a, b) =>
                {
                    return a.Priority.CompareTo(b.Priority);
                });
            }
            else
            {
                SubscribersByType.Add(handle, new List<PrioritySubscriber>
                {
                    new PrioritySubscriber
                    {
                        Subscriber = subscriber,
                        Priority = prioity
                    }
                });
            }
        }

        public void UnSubscribe<S>(S subscriber) where S : IEventBusSubscriber
        {
            Type type = typeof(S);
            RuntimeTypeHandle handle = type.TypeHandle;
            if (SubscribersByType.ContainsKey(handle))
            {
                var sub = SubscribersByType[handle].FirstOrDefault(s => s.Subscriber.GetHashCode() == subscriber.GetHashCode());
                if (sub != null)
                {
                    SubscribersByType[handle].Remove(sub);
                }
            }
        }

        private class PrioritySubscriber
        {
            public IEventBusSubscriber Subscriber;
            public int Priority;
        }
    } 
}
