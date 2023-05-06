using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRoundManager : BaseRoundManager
{
    public SingleRoundManager(ScoreMenu scoreMenu, Hand[] hands)
    : base(Constants.SingleRoundManager.MaxRounds, scoreMenu, hands)
    {
        var heartsRound = new HeartsRound(this.roundContext);
        this.roundContext.SetState(heartsRound);
        this.gameStateContext.Start();
    }
}
