using System.Collections;
using System.Collections.Generic;

public class ChaosRoundManager : BaseRoundManager
{
    public ChaosRoundManager(ScoreMenu scoreMenu, Hand[] hands)
    : base(Constants.ChaosRoundManager.MaxRounds, scoreMenu, hands)
    {
        var nothingRound = new NothingRound(this.roundContext);
        this.roundContext.SetState(nothingRound);
        this.gameStateContext.Start();
    }
}
