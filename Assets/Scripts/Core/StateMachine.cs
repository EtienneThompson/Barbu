namespace Barbu.Core
{
    using Barbu.Core.Events;
    using Barbu.Gameplay;

    public class StateMachine : IStateMachine
    {
        public StateBool IsSettingUp { get; private set; } = false;
        public StateInteger NumCardsPlayed { get; private set; } = 0;
        public StateBool CanCardBePlayed { get; private set; } = false;
        public StateBool IsMenuOpen { get; private set; } = false;
        public StateInteger HighestRank { get; private set; } = 0;
        public StateBool IsGamePaused { get; private set; } = false;
        public StateBool AutoPlayCards { get; private set; } = false;

        private string startingSuit = string.Empty;
        private bool playerCardMustBeStartingSuit;

        private readonly IEventsController eventsController;

        public StateMachine(IEventsController eventsController)
        {
            this.eventsController = eventsController;
        }

        public string GetStartingSuit()
        {
            return this.startingSuit;
        }

        public void SetStartingSuit(string suit)
        {
            this.startingSuit = suit;
        }

        public void SetPlayerMustPlayStartingSuit(bool isStartingPlayer, Hand hand)
        {
            this.playerCardMustBeStartingSuit = !isStartingPlayer && hand.CardsInSuit(this.startingSuit).Count > 0;
        }

        public bool MustPlayCardInStartingSuit()
        {
            return this.playerCardMustBeStartingSuit;
        }

        public void SetMenuOpen(bool isOpen)
        {
            this.IsMenuOpen.Set(isOpen);

            if (isOpen)
            {
                this.IsGamePaused.Enable();
                this.eventsController.Fire(EventNames.PauseGame);
            }
            else
            {
                this.IsGamePaused.Disable();
                this.eventsController.Fire(EventNames.ResumeGame);
            }
        }
    }
}
