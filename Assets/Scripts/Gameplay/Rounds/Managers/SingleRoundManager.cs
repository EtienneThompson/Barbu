using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRoundManager : BaseRoundManager
{
    public SingleRoundManager(GameBoard gameBoard, ScoreMenu scoreMenu, InGamePointsController inGamePointsController, Hand[] hands, string subType)
    : base(Constants.SingleRoundManager.MaxRounds, gameBoard, scoreMenu, inGamePointsController, hands)
    {
        Round round;
        switch (subType)
        {
            case Constants.SingleRoundManager.Hearts:
                round = new HeartsRound(this.roundContext);
                break;
            case Constants.SingleRoundManager.Queens:
                round = new QueensRound(this.roundContext);
                break;
            case Constants.SingleRoundManager.KingOfHearts:
                round = new KingOfHeartsRound(this.roundContext);
                break;
            case Constants.SingleRoundManager.Piles:
                round = new PilesRound(this.roundContext);
                break;
            case Constants.SingleRoundManager.Nothing:
                round = new NothingRound(this.roundContext);
                break;
            case Constants.SingleRoundManager.Everything:
                round = new EverythingRound(this.roundContext);
                break;
            default:
                throw new System.Exception("Unknown single round provided");
        }

        this.roundContext.SetState(round);
        this.gameStateContext.Start();
    }

    protected override void MarkGameAsFinished()
    {
        Statistics.IncrementGamesFinished(Statistics.GameTypes.Single);
    }

    protected override void MarkGameAsWon()
    {
        Statistics.IncrementGamesWon(Statistics.GameTypes.Single);
    }
}
