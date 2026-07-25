namespace Barbu.UI.Controllers
{
    public interface IRoundOverlayController
    {
        void DisplayRound(string roundName, GameTypes gameType);

        void DisplayWinner(string[] winningPlayers);

        void ShowRoundOverMessage();

        void HideText();

        void SetActive(bool active);
    }
}
