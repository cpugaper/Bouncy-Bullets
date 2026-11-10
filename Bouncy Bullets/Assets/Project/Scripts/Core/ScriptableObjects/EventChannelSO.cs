using System;
using UnityEngine;

namespace Game.Core
{
    public abstract class EventChannelSO<T> : ScriptableObject
    {
        private Action<T> onEventRaised;

        public void Subscribe(Action<T> listener) => onEventRaised += listener;
        public void Unsubscribe(Action<T> listener) => onEventRaised -= listener;

        public void RaiseEvent(T position) => onEventRaised?.Invoke(position);
    }
}
