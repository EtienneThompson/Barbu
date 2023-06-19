using System.Collections;
using System.Collections.Generic;
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
        this.gameStateContext.Start();
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
