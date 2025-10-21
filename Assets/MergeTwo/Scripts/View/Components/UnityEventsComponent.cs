using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MergeTwo
{
    public class UnityEventsComponent : MonoBehaviour
    {
        [SerializeField] Events _events;

        void Awake()
        {
            _events.OnAwake.Invoke();
        }

        IEnumerator Start()
        {
            _events.OnStart.Invoke();
            yield return null;
            _events.OnStartNextFrame.Invoke();
        }

        void OnEnable()
        {
            _events.OnEnable.Invoke();
        }

        void OnDisable()
        {
            _events.OnDisable.Invoke();
        }

        void OnDestroy()
        {
            _events.OnDestroy.Invoke();
        }

        [Serializable]
        private class Events
        {
            public UnityEvent OnAwake;
            public UnityEvent OnStart;
            public UnityEvent OnStartNextFrame;
            public UnityEvent OnEnable;
            public UnityEvent OnDisable;
            public UnityEvent OnDestroy;
        }
    } 
}
