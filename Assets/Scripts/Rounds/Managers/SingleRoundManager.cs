using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRoundManager : BaseRoundManager
{
    public SingleRoundManager(GameBoard gameBoard, ScoreMenu scoreMenu, Hand[] hands)
    : base(Constants.SingleRoundManager.MaxRounds, gameBoard, scoreMenu, hands)
    {
        var heartsRound = new PilesRound(this.roundContext);
        this.roundContext.SetState(heartsRound);
        this.gameStateContext.Start();
    }
}
