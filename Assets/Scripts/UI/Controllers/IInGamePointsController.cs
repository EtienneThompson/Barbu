namespace Barbu.UI.Controllers
{
    public interface IInGamePointsController
    {
        void SetRoundName(string roundName);

        void ResetRoundName();

        void UpdatePlayerPoints(string player, int points);

        void ResetPoints();

        void SetActive(bool active);
    }
}
