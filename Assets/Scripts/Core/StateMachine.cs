namespace Barbu.Core
{
    using Barbu.Gameplay;
    using Barbu.Models;

    public class StateMachine
    {
        private static readonly StateMachine instance = new StateMachine();

        private bool isSettingUp = false;
        private int numCardsPlayed = 0;
        private bool canCardBePlayed = false;
        private string startingSuit = string.Empty;
        private bool menuOpen = false;
        private int highestRank = 0;
        private bool isGamePaused = false;

        private bool playerCardMustBeStartingSuit;

        private EventsController eventsController;

        public StateMachine()
        {
            this.eventsController = EventsController.GetInstance();
        }

        public void SetIsSettingUp(bool isSetup)
        {
            instance.isSettingUp = isSetup;
        }

        public bool IsSettingUp()
        {
            return instance.isSettingUp;
        }

        public int NumCardsPlayed()
        {
            return instance.numCardsPlayed;
        }

        public bool IsCardPlayable()
        {
            return instance.canCardBePlayed;
        }

        public void IncrementNumCardsPlayed()
        {
            instance.numCardsPlayed += 1;
        }

        public void ResetNumCardsPlayed()
        {
            instance.numCardsPlayed = 0;
        }

        public void SetCardPlayable(bool state)
        {
            instance.canCardBePlayed = state;
        }

        public string GetStartingSuit()
        {
            return instance.startingSuit;
        }

        public void SetStartingSuit(string suit)
        {
            instance.startingSuit = suit;
        }

        public void SetPlayerMustPlayStartingSuit(bool isStartingPlayer, Hand hand)
        {
            instance.playerCardMustBeStartingSuit = !isStartingPlayer && hand.CardsInSuit(instance.startingSuit).Count > 0;
        }

        public bool MustPlayCardInStartingSuit()
        {
            return instance.playerCardMustBeStartingSuit;
        }

        public bool IsMenuOpen()
        {
            return instance.menuOpen;
        }

        public void SetMenuOpen(bool isOpen)
        {
            instance.menuOpen = isOpen;

            if (isOpen)
            {
                instance.isGamePaused = true;
                this.eventsController.Fire(EventNames.PauseGame);
            }
            else
            {
                instance.isGamePaused = false;
                this.eventsController.Fire(EventNames.ResumeGame);
            }
        }

        public int GetHighestRankedCard()
        {
            return instance.highestRank;
        }

        public void SetHighestRank(int rank)
        {
            instance.highestRank = rank;
        }

        public bool IsGamePaused()
        {
            return instance.isGamePaused;
        }
    }
}
