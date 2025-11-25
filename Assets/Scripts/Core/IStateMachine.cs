namespace Barbu.Core
{
    using Barbu.Gameplay;

    public interface IStateMachine
    {
        StateInteger NumCardsPlayed { get; }
        StateInteger HighestRank { get; }
        StateBool IsSettingUp { get; }
        StateBool CanCardBePlayed { get; }
        StateBool AutoPlayCards { get; }
        StateBool IsMenuOpen { get; }
        StateBool IsGamePaused { get; }

        string GetStartingSuit();
        void SetStartingSuit(string suit);
        void SetPlayerMustPlayStartingSuit(bool isStartingPlayer, Hand hand);
        bool MustPlayCardInStartingSuit();
        void SetMenuOpen(bool isMenuOpen);
    }
}
