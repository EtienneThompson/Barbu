namespace Barbu.Core
{
    using System;
    using Barbu.Models;

    public interface IEventsController
    {
        void Subscribe(EventNames eventName, Action<EventNames> listener);
        void Subscribe(EventNames eventName, Action<EventNames, object> listener);
        void Unsubscribe(EventNames eventName, Action<EventNames> listener);
        void Unsubscribe(EventNames eventName, Action<EventNames, object> listener);
        void Fire(EventNames eventName);
        void Fire<T>(EventNames eventName, T data);
    }
}
