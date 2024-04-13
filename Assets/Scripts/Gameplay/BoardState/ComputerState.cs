namespace Barbu.Gameplay.BoardState
{
    using System;
    using Barbu.Gameplay;

    public class ComputerState : GameState
    {
        public ComputerState(GameStateContext context, string id, Hand hand)
        : base(context, hand, id)
        {
        }

        public ComputerState(GameStateContext context, GameState next, string id, Hand hand)
        : base(context, next, hand, id)
        {
        }

        public override void Start()
        {
            if (!stateMachine.IsCardPlayable())
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
                // If no cards in hand of the same suit, then pick a random one.
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
