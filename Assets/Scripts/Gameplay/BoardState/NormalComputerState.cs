namespace Barbu.Gameplay.BoardState
{
    using System;
    using Barbu.Core;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;

    public class NormalComputerState : GameState
    {
        public NormalComputerState(
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IRound round,
            string id,
            Hand hand)
        : base(stateMachine, telemetryService, round, hand, id)
        {
        }

        public override void Start()
        {
            if (!this.stateMachine.CanCardBePlayed)
            {
                throw new Exception("Computer can't a move right now");
            }

            this.stateMachine.CanCardBePlayed.Enable();
            var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
            var playableCards = cardsInSuit.Count > 0 ? cardsInSuit : this.hand.GetAvailableCards();

            Card lowestCard = null;
            Card highestCard = null;
            foreach (var card in playableCards)
            {
                if (card.rank < (lowestCard?.rank ?? int.MaxValue))
                {
                    lowestCard = card;
                }

                if (card.rank > (highestCard?.rank ?? int.MinValue))
                {
                    highestCard = card;
                }
            }

            if (this.round.IsRoundPositive())
            {
                highestCard.PlayCard();
            }
            else
            {
                lowestCard.PlayCard();
            }
        }
    }
}