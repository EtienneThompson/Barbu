namespace Barbu.Gameplay.BoardState
{
    using Barbu.Core;
    using Barbu.Gameplay;
    using Barbu.Interfaces.BoardState;
    using Barbu.Interfaces.Rounds;

    public abstract class GameState : IGameState
    {
        protected IRound round;
        protected Hand hand;
        protected IStateMachine stateMachine;
        protected ITelemetryService telemetryService;

        public string PlayerId { get; private set; }

        public GameState(
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IRound round,
            Hand hand,
            string id)
        {
            this.round = round;
            this.hand = hand;
            this.PlayerId = id;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public virtual void Start()
        {
            // Do nothing, this will be overridden by states appropriately.
        }

        public virtual void CleanUp()
        {
            // Always adjust the position of the cards in the hand.
            int index = 0;
            var availableCards = this.hand.GetAvailableCards();
            var cardsInHand = availableCards.Count;
            foreach (var card in availableCards)
            {
                card.StartPositionAdjustment(index, cardsInHand);
                index++;
            }
        }
    }
}
