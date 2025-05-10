namespace Barbu.Gameplay.BoardState
{
    using System;
    using Barbu.Core;
    using Barbu.Core.Telemetry;
    using Barbu.Gameplay;
    using Barbu.Gameplay.Rounds;

    public class EasyComputerState : GameState
    {
        public EasyComputerState(
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
            if (!this.stateMachine.IsCardPlayable())
            {
                throw new Exception("Computer can't make a move right now.");
            }

            this.stateMachine.SetCardPlayable(false);
            var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
            if (cardsInSuit.Count > 0)
            {
                foreach (var card in cardsInSuit)
                {
                    if (card.state == Card.CardState.Waiting)
                    {
                        card.PlayCard();
                        return;
                    }
                }
            }
            else
            {
                foreach (var card in this.hand.GetHand())
                {
                    if (card.state == Card.CardState.Waiting)
                    {
                        card.PlayCard();
                        return;
                    }
                }
            }
        }
    }
}
