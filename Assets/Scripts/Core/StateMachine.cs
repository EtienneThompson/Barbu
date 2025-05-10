namespace Barbu.Core
{
    using Barbu.Gameplay;
    using Barbu.Models;

    public class StateMachine : IStateMachine
    {
        private bool isSettingUp = false;
        private int numCardsPlayed = 0;
        private bool canCardBePlayed = false;
        private string startingSuit = string.Empty;
        private bool menuOpen = false;
        private int highestRank = 0;
        private bool isGamePaused = false;
        private bool AutoPlayCards = false;

        private bool playerCardMustBeStartingSuit;

        private readonly IEventsController eventsController;

        public StateMachine(IEventsController eventsController)
        {
            this.eventsController = eventsController;
        }

        public void SetIsSettingUp(bool isSetup)
        {
            this.isSettingUp = isSetup;
        }

        public bool IsSettingUp()
        {
            return this.isSettingUp;
        }

        public int NumCardsPlayed()
        {
            return this.numCardsPlayed;
        }

        public bool IsCardPlayable()
        {
            return this.canCardBePlayed;
        }

        public void IncrementNumCardsPlayed()
        {
            this.numCardsPlayed += 1;
        }

        public void ResetNumCardsPlayed()
        {
            this.numCardsPlayed = 0;
        }

        public void SetCardPlayable(bool state)
        {
            this.canCardBePlayed = state;
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

        public bool IsMenuOpen()
        {
            return this.menuOpen;
        }

        public void SetMenuOpen(bool isOpen)
        {
            this.menuOpen = isOpen;

            if (isOpen)
            {
                this.isGamePaused = true;
                this.eventsController.Fire(EventNames.PauseGame);
            }
            else
            {
                this.isGamePaused = false;
                this.eventsController.Fire(EventNames.ResumeGame);
            }
        }

        public int GetHighestRankedCard()
        {
            return this.highestRank;
        }

        public void SetHighestRank(int rank)
        {
            this.highestRank = rank;
        }

        public bool IsGamePaused()
        {
            return this.isGamePaused;
        }

        public void SetAutoPlayMode(bool autoPlayMode)
        {
            this.AutoPlayCards = autoPlayMode;
        }

        public bool IsAutoPlayMode()
        {
            return this.AutoPlayCards;
        }
    }
}
