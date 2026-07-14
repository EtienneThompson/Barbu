namespace Barbu.Tests.EditMode.TestUtils
{
    using Barbu.Core;
    using Barbu.Gameplay;

    /// <summary>
    /// Minimal, hand-rolled IStateMachine used to isolate tests from the real
    /// StateMachine's event-firing side effects (e.g. SetMenuOpen).
    /// </summary>
    public class FakeStateMachine : IStateMachine
    {
        private bool isSettingUp;
        private int numCardsPlayed;
        private bool canCardBePlayed;
        private string startingSuit = string.Empty;
        private bool menuOpen;
        private int highestRank;
        private bool isGamePaused;
        private bool autoPlayMode;
        private bool mustPlayStartingSuit;

        public void SetIsSettingUp(bool isSetup) => this.isSettingUp = isSetup;

        public bool IsSettingUp() => this.isSettingUp;

        public int NumCardsPlayed() => this.numCardsPlayed;

        public bool IsCardPlayable() => this.canCardBePlayed;

        public void IncrementNumCardsPlayed() => this.numCardsPlayed++;

        public void ResetNumCardsPlayed() => this.numCardsPlayed = 0;

        public void SetCardPlayable(bool state) => this.canCardBePlayed = state;

        public string GetStartingSuit() => this.startingSuit;

        public void SetStartingSuit(string suit) => this.startingSuit = suit;

        public void SetPlayerMustPlayStartingSuit(bool isStartingPlayer, Hand hand) =>
            this.mustPlayStartingSuit = !isStartingPlayer && hand.CardsInSuit(this.startingSuit).Count > 0;

        public bool MustPlayCardInStartingSuit() => this.mustPlayStartingSuit;

        public bool IsMenuOpen() => this.menuOpen;

        public void SetMenuOpen(bool isMenuOpen)
        {
            this.menuOpen = isMenuOpen;
            this.isGamePaused = isMenuOpen;
        }

        public int GetHighestRankedCard() => this.highestRank;

        public void SetHighestRank(int rank) => this.highestRank = rank;

        public bool IsGamePaused() => this.isGamePaused;

        public void SetAutoPlayMode(bool autoPlayMode) => this.autoPlayMode = autoPlayMode;

        public bool IsAutoPlayMode() => this.autoPlayMode;
    }
}
