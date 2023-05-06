using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TraditionalRoundManager : BaseRoundManager
{
    public TraditionalRoundManager(ScoreMenu scoreMenu, Hand[] hands)
    : base(Constants.TraditionalRoundManager.MaxRounds, scoreMenu, hands)
    {
        var everythingRound = new EverythingRound(this.roundContext);
        var nothingRound = new NothingRound(this.roundContext, everythingRound);
        var pilesRound = new PilesRound(this.roundContext, nothingRound);
        var kingOfHeartsRound = new KingOfHeartsRound(this.roundContext, pilesRound);
        var queensRound = new QueensRound(this.roundContext, kingOfHeartsRound);
        var heartsRound = new HeartsRound(this.roundContext, queensRound);
        this.roundContext.SetState(heartsRound);

        this.gameStateContext.Start();
    }
}
