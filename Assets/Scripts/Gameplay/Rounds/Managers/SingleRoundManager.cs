using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRoundManager : BaseRoundManager
{
    public SingleRoundManager(GameBoard gameBoard, ScoreMenu scoreMenu, InGamePointsController inGamePointsController, Hand[] hands)
    : base(Constants.SingleRoundManager.MaxRounds, gameBoard, scoreMenu, inGamePointsController, hands)
    {
        var heartsRound = new HeartsRound(this.roundContext);
        this.roundContext.SetState(heartsRound);
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
