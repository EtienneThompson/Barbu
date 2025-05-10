namespace Barbu.Gameplay
{
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Models;

    public class Pile
    {
        private List<Card> cardsInPile;
        private int cardsResolved;
        private readonly IStateMachine stateMachine;
        private readonly IEventsController eventsController;

        public Pile(IStateMachine stateMachine, IEventsController eventsController)
        {
            this.cardsInPile = new List<Card>();
            this.cardsResolved = 0;
            this.stateMachine = stateMachine;
            this.eventsController = eventsController;
        }

        public int GetPileSize()
        {
            return this.cardsInPile.Count;
        }

        public Card GetHighestCard()
        {
            if (this.cardsInPile.Count == 0)
            {
                return null;
            }

            var highestCardIndex = 0;
            for (int i = 0; i < this.cardsInPile.Count; i++)
            {
                if (this.cardsInPile[i] == null)
                {
                    continue;
                }

                if (this.cardsInPile[i].suit == this.stateMachine.GetStartingSuit() &&
                    this.cardsInPile[i].rank > this.cardsInPile[highestCardIndex].rank)
                {
                    highestCardIndex = i;
                }
            }

            return this.cardsInPile[highestCardIndex];
        }

        public List<Card> GetCards()
        {
            return this.cardsInPile;
        }

        public void AddCardToPile(Card card)
        {
            this.cardsInPile.Add(card);
            if (this.cardsInPile.Count == 1)
            {
                this.stateMachine.SetStartingSuit(card.suit);
            }
        }

        public void StartPileResolution(string winningPlayer)
        {
            this.eventsController.Subscribe(EventNames.CardInPileResolved, this.OnCardInPileResolved);
            foreach (var card in this.cardsInPile)
            {
                card.StartPileResolution(winningPlayer);
            }
        }

        private void OnCardInPileResolved(EventNames _)
        {
            this.cardsResolved++;
            if (this.cardsResolved == 4)
            {
                this.eventsController.Unsubscribe(EventNames.CardInPileResolved, this.OnCardInPileResolved);
                this.eventsController.Fire(EventNames.PileResolved);
            }
        }
    }
}
