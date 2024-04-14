namespace Barbu.Gameplay.BoardState
{
    using System;
    using Barbu.Gameplay;

    public class NormalComputerState : GameState
    {
        public NormalComputerState(GameStateContext context, string id, Hand hand)
        : base(context, hand, id)
        {
        }

        public NormalComputerState(GameStateContext context, GameState next, string id, Hand hand)
        : base(context, next, hand, id)
        {
        }

        public override void Start()
        {
            if (!this.stateMachine.IsCardPlayable())
            {
                throw new Exception("Computer can't a move right now");
            }

            this.stateMachine.SetCardPlayable(true);
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

            if (this.context.IsCurrentRoundPositive())
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