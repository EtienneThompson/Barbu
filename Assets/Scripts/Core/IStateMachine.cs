namespace Barbu.Core
{
    using Barbu.Gameplay;

    public interface IStateMachine
    {
        void SetIsSettingUp(bool isSetup);
        bool IsSettingUp();
        int NumCardsPlayed();
        bool IsCardPlayable();
        void IncrementNumCardsPlayed();
        void ResetNumCardsPlayed();
        void SetCardPlayable(bool state);
        string GetStartingSuit();
        void SetStartingSuit(string suit);
        void SetPlayerMustPlayStartingSuit(bool isStartingPlayer, Hand hand);
        bool MustPlayCardInStartingSuit();
        bool IsMenuOpen();
        void SetMenuOpen(bool isMenuOpen);
        int GetHighestRankedCard();
        void SetHighestRank(int rank);
        bool IsGamePaused();
        void SetAutoPlayMode(bool autoPlayMode);
        bool IsAutoPlayMode();
    }
}
