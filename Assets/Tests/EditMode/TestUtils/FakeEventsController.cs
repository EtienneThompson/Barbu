namespace Barbu.Tests.EditMode.TestUtils
{
    using System;
    using System.Collections.Generic;
    using Barbu.Core.Events;

    /// <summary>
    /// No-op IEventsController that records fired events so tests can assert on
    /// them without pulling in the real event bus.
    /// </summary>
    public class FakeEventsController : IEventsController
    {
        public List<EventNames> FiredEvents { get; } = new();

        public List<EventNames> SubscribedEvents { get; } = new();

        public List<EventNames> UnsubscribedEvents { get; } = new();

        public void Subscribe(EventNames eventName, Action<EventNames> listener)
        {
            this.SubscribedEvents.Add(eventName);
        }

        public void Subscribe(EventNames eventName, Action<EventNames, object> listener)
        {
            this.SubscribedEvents.Add(eventName);
        }

        public void Unsubscribe(EventNames eventName, Action<EventNames> listener)
        {
            this.UnsubscribedEvents.Add(eventName);
        }

        public void Unsubscribe(EventNames eventName, Action<EventNames, object> listener)
        {
            this.UnsubscribedEvents.Add(eventName);
        }

        public void Fire(EventNames eventName)
        {
            this.FiredEvents.Add(eventName);
        }

        public void Fire<T>(EventNames eventName, T data)
        {
            this.FiredEvents.Add(eventName);
        }
    }
}
