namespace Barbu
{
    using Barbu.Gameplay;
    using UnityEngine;
    using Zenject;

    public class CardFactory : ICardFactory
    {
        private readonly IInstantiator instantiator;

        public CardFactory(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public Card CreateCard(string suit, string rank, string playerId)
        {
            GameObject cardObject = this.instantiator.InstantiatePrefabResource("BlankPlayingCard");
            Card card = cardObject.GetComponent<Card>();

            card.InitializeData(suit, rank, playerId);
            return card;
        }
    }
}
