namespace Barbu.Gameplay.BoardState
{
    using Barbu.Core;
    using Barbu.Gameplay;
    using Barbu.Interfaces.Rounds;
    using UnityEngine;

    public class PlayerState : GameState
    {
        public PlayerState(
            IStateMachine stateMachine,
            ITelemetryService telemetryService,
            IRound round, string id, Hand hand)
        : base(stateMachine, telemetryService, round, hand, id)
        {
        }

        public override void Start()
        {
            // Determine if the player must play a card in the starting suite based
            // on if they are the first player in a round and the cards in their hand.
            var startingPlayer = string.IsNullOrEmpty(this.stateMachine.GetStartingSuit());
            this.stateMachine.SetPlayerMustPlayStartingSuit(startingPlayer, this.hand);

            // Highlight all cards that are of the starting suit.
            var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
            if (cardsInSuit.Count == 0)
            {
                cardsInSuit = this.hand.GetAvailableCards();
            }
            foreach (var card in cardsInSuit)
            {
                card.Highlight(Color.yellow);
            }
        }

        public override void CleanUp()
        {
            // When the player has finished and we should move on to the next state,
            // remove all highlights.
            foreach (var card in this.hand.GetHand())
            {
                card.RemoveHighlight();
            }

            base.CleanUp();
        }
    }
}