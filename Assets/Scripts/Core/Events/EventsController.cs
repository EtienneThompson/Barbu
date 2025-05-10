namespace Barbu.Core.Events
{
    using System;
    using Barbu.Core.Telemetry;

    /// <summary>
    /// EventsController is the main hub at which different systems in the game can
    /// subscribe to receive events from other systems.
    ///
    /// For example, the gameplay loop system can subscribe to the pause or resume
    /// delegates to be told when to stop by another system, i.e. the menuing system.
    /// </summary>
    public class EventsController : IEventsController
    {
        public static event Action<EventNames> PauseGame;
        public static event Action<EventNames> ResumeGame;
        public static event Action<EventNames, object> PlayCard;
        public static event Action<EventNames> ScoreMenuDismissed;
        public static event Action<EventNames> CardInPileResolved;
        public static event Action<EventNames> PileResolved;
        public static event Action<EventNames> RoundAnimationOver;
        public static event Action<EventNames> WinnerAnimationOver;

        private readonly ITelemetryService telemetryService;

        public EventsController(ITelemetryService telemetryService)
        {
            this.telemetryService = telemetryService;
        }

        public void Subscribe(EventNames eventName, Action<EventNames> listener)
        {
            this.telemetryService.LogInfo($"Subscribing to event {eventName} with listener");
            switch (eventName)
            {
                case EventNames.PauseGame:
                    PauseGame += listener;
                    return;
                case EventNames.ResumeGame:
                    ResumeGame += listener;
                    return;
                case EventNames.ScoreMenuDismissed:
                    ScoreMenuDismissed += listener;
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
                    var errorMessage = $"The event {eventName} is not able to be subscribed to!";
                    this.telemetryService.LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
            }
        }

        public void Subscribe(EventNames eventName, Action<EventNames, object> listener)
        {
            this.telemetryService.LogInfo($"Subscribing to event {eventName} with data listener");
            switch (eventName)
            {
                case EventNames.PlayCard:
                    PlayCard += listener;
                    return;
                default:
                    var errorMessage = $"The event {eventName} is not able to be subscribed to!";
                    this.telemetryService.LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
            }
        }

        public void Unsubscribe(EventNames eventName, Action<EventNames> listener)
        {
            this.telemetryService.LogInfo($"Unsubscribing from event {eventName} with listener");
            switch (eventName)
            {
                case EventNames.PauseGame:
                    PauseGame -= listener;
                    return;
                case EventNames.ResumeGame:
                    ResumeGame -= listener;
                    return;
                case EventNames.ScoreMenuDismissed:
                    ScoreMenuDismissed -= listener;
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
                    var errorMessage = $"The event {eventName} is not able to be unsubscribed from!";
                    this.telemetryService.LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
            }
        }

        public void Unsubscribe(EventNames eventName, Action<EventNames, object> listener)
        {
            this.telemetryService.LogInfo($"Unsubscribing from event {eventName} with data listener");
            switch (eventName)
            {
                case EventNames.PlayCard:
                    PlayCard -= listener;
                    return;
                default:
                    var errorMessage = $"The event {eventName} is not able to be unsubscribed from!";
                    this.telemetryService.LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
            }
        }

        public void Fire(EventNames eventName)
        {
            this.telemetryService.LogInfo($"Firing event {eventName}");
            switch (eventName)
            {
                case EventNames.PauseGame:
                    PauseGame.Invoke(EventNames.PauseGame);
                    return;
                case EventNames.ResumeGame:
                    ResumeGame.Invoke(EventNames.ResumeGame);
                    return;
                case EventNames.ScoreMenuDismissed:
                    ScoreMenuDismissed.Invoke(EventNames.ScoreMenuDismissed);
                    return;
                case EventNames.CardInPileResolved:
                    CardInPileResolved.Invoke(EventNames.CardInPileResolved);
                    return;
                case EventNames.PileResolved:
                    PileResolved.Invoke(EventNames.PileResolved);
                    return;
                case EventNames.RoundAnimationFinished:
                    RoundAnimationOver.Invoke(EventNames.RoundAnimationFinished);
                    return;
                case EventNames.WinnerAnimationFinished:
                    WinnerAnimationOver.Invoke(EventNames.WinnerAnimationFinished);
                    return;
                default:
                    var errorMessage = $"The event {eventName} is not able to be subscribed to!";
                    this.telemetryService.LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
            }
        }

        public void Fire<T>(EventNames eventName, T data)
        {
            this.telemetryService.LogInfo($"Firing event {eventName} with data {data}");
            switch (eventName)
            {
                case EventNames.PlayCard:
                    PlayCard.Invoke(EventNames.PlayCard, data);
                    return;
                default:
                    var errorMessage = $"The event {eventName} is not able to be fired!";
                    this.telemetryService.LogError(errorMessage);
                    throw new ArgumentException(errorMessage);
            }
        }
    }
}
