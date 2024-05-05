namespace Barbu.Gameplay.Rounds.Managers
{
    using Barbu.Gameplay.Rounds.Rounds;
    using Barbu.UI.Controllers;

    public class TraditionalRoundManager : BaseRoundManager
    {
        public TraditionalRoundManager(GameBoard gameBoard, ScoreMenu scoreMenu, InGamePointsController inGamePointsController, Hand[] hands)
        : base(Constants.TraditionalRoundManager.MaxRounds, gameBoard, scoreMenu, inGamePointsController, hands)
        {
            var everythingRound = new EverythingRound(this.roundContext);
            var nothingRound = new NothingRound(this.roundContext, everythingRound);
            var pilesRound = new PilesRound(this.roundContext, nothingRound);
            var kingOfHeartsRound = new KingOfHeartsRound(this.roundContext, pilesRound);
            var queensRound = new QueensRound(this.roundContext, kingOfHeartsRound);
            var heartsRound = new HeartsRound(this.roundContext, queensRound);
            this.roundContext.SetState(heartsRound);
        }

        protected override void MarkGameAsFinished()
        {
            Statistics.IncrementGamesFinished(Statistics.GameTypes.Traditional);
        }

        protected override void MarkGameAsWon()
        {
            Statistics.IncrementGamesWon(Statistics.GameTypes.Traditional);
        }
    }
}
