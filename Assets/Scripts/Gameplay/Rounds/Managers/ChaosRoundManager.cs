namespace Barbu.Gameplay.Rounds.Managers
{
    using Barbu.Gameplay.Rounds;
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.UI.Controllers;
    using UnityEngine;

    public class ChaosRoundManager : BaseRoundManager
    {
        public ChaosRoundManager(GameBoard gameBoard, ScoreMenu scoreMenu, InGamePointsController inGamePointsController, Hand[] hands)
        : base(Constants.ChaosRoundManager.MaxRounds, gameBoard, scoreMenu, inGamePointsController, hands)
        {
            var chaosRound = new ChaosRound(this.roundContext);

            var roundsToMerge = (int)Mathf.Floor(UnityEngine.Random.Range(2.0f, 4.0f));
            for (int i = 0; i < roundsToMerge; i++)
            {
                var round = RoundRegistration.GetRandomRound();
                chaosRound.MergeRound(round);
            }

            this.roundContext.SetState(chaosRound);
        }

        protected override void MarkGameAsFinished()
        {
            Statistics.IncrementGamesFinished(Statistics.GameTypes.Chaos);
        }

        protected override void MarkGameAsWon()
        {
            Statistics.IncrementGamesWon(Statistics.GameTypes.Chaos);
        }
    }
}