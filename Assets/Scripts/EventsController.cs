namespace Barbu
{
    using System;
    using Barbu.Models;
    using Newtonsoft.Json;


    /// <summary>
    /// EventsController is the main hub at which different systems in the game can
    /// subscribe to receive events from other systems.
    ///
    /// For example, the gameplay loop system can subscribe to the pause or resume
    /// delegates to be told when to stop by another system, i.e. the menuing system.
    /// </summary>
    public class EventsController
    {
        public static event Action PauseGame;
        public static event Action ResumeGame;
        public static event Action<object> PlayCard;
        public static event Action EndRound;
        public static event Action CardInPileResolved;
        public static event Action PileResolved;
        public static event Action RoundAnimationOver;
        public static event Action WinnerAnimationOver;

        private static EventsController singleton;

        private EventsController()
        {
        }

        public static EventsController GetInstance()
        {
            if (singleton == null)
            {
                singleton = new EventsController();
            }

            return singleton;
        }

        public void Subscribe(EventNames eventName, Action listener)
        {
            UnityEngine.Debug.Log($"Subscribing to event {eventName}");
            switch (eventName)
            {
                case EventNames.PauseGame:
                    PauseGame += listener;
                    return;
                case EventNames.ResumeGame:
                    ResumeGame += listener;
                    return;
                case EventNames.RoundOver:
                    EndRound += listener;
                    return;
                case EventNames.CardInPileResolved:
                    CardInPileResolved += listener;
                    return;
                case EventNames.PileResolved:
                    PileResolved += listener;
                    return;
                case EventNames.RoundAnimationFinished:
                    RoundAnimationOver += listener;
                    return;
                case EventNames.WinnerAnimationFinished:
                    WinnerAnimationOver += listener;
                    return;
                default:
                    throw new ArgumentException($"The event {eventName} is not able to be subscribed to!");
            }
        }

        public void Subscribe(EventNames eventName, Action<object> listener)
        {
            UnityEngine.Debug.Log($"Subscribing action {nameof(listener)} to event {eventName}");
            switch (eventName)
            {
                case EventNames.PlayCard:
                    PlayCard += listener;
                    return;
                default:
                    throw new ArgumentException($"The event {eventName} is not able to be subscribed to!");
            }
        }

        public void Unsubscribe(EventNames eventName, Action listener)
        {
            UnityEngine.Debug.Log($"Unsubscribing action {nameof(listener)} from event {eventName}");
            switch (eventName)
            {
                case EventNames.PauseGame:
                    PauseGame -= listener;
                    return;
                case EventNames.ResumeGame:
                    ResumeGame -= listener;
                    return;
                case EventNames.RoundOver:
                    EndRound -= listener;
                    return;
                case EventNames.CardInPileResolved:
                    CardInPileResolved -= listener;
                    return;
                case EventNames.PileResolved:
                    PileResolved -= listener;
                    return;
                case EventNames.RoundAnimationFinished:
                    RoundAnimationOver -= listener;
                    return;
                case EventNames.WinnerAnimationFinished:
                    WinnerAnimationOver -= listener;
                    return;
                default:
                    throw new ArgumentException($"The event {eventName} is not able to be unsubscribed from!");
            }
        }

        public void Unsubscribe(EventNames eventName, Action<object> listener)
        {
            UnityEngine.Debug.Log($"Unsubscribing action {nameof(listener)} from event {eventName}");
            switch (eventName)
            {
                case EventNames.PlayCard:
                    PlayCard -= listener;
                    return;
                default:
                    throw new ArgumentException($"The event {eventName} is not able to be unsubscribed from!");
            }
        }

        public void Fire(EventNames eventName)
        {
            UnityEngine.Debug.Log($"Firing event {eventName}");
            switch (eventName)
            {
                case EventNames.PauseGame:
                    PauseGame.Invoke();
                    return;
                case EventNames.ResumeGame:
                    ResumeGame.Invoke();
                    return;
                case EventNames.RoundOver:
                    EndRound.Invoke();
                    return;
                case EventNames.CardInPileResolved:
                    CardInPileResolved.Invoke();
                    return;
                case EventNames.PileResolved:
                    PileResolved.Invoke();
                    return;
                case EventNames.RoundAnimationFinished:
                    RoundAnimationOver.Invoke();
                    return;
                case EventNames.WinnerAnimationFinished:
                    WinnerAnimationOver.Invoke();
                    return;
                default:
                    throw new ArgumentException($"The event {eventName} is not able to be subscribed to!");
            }
        }

        public void Fire<T>(EventNames eventName, T data)
        {
            UnityEngine.Debug.Log($"Firing event ${eventName} with data ${data}");
            switch (eventName)
            {
                case EventNames.PlayCard:
                    PlayCard.Invoke(data);
                    return;
                default:
                    throw new ArgumentException($"The event {eventName} is not able to be fired!");
            }
        }
    }
}
