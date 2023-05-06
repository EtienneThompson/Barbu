using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TraditionalRoundManager : BaseRoundManager
{
    public TraditionalRoundManager(int numRounds, ScoreMenu scoreMenu, Hand[] hands)
    : base(scoreMenu, hands)
    {
        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, "1", hands[0]);
        var computerState3 = new ComputerState(this.gameStateContext, playerState, "4", hands[3]);
        var computerState2 = new ComputerState(this.gameStateContext, computerState3, "3", hands[2]);
        var computerState1 = new ComputerState(this.gameStateContext, computerState2, "2", hands[1]);
        playerState.SetNextState(computerState1);

        this.players[0] = playerState;
        this.players[1] = computerState1;
        this.players[2] = computerState2;
        this.players[3] = computerState3;

        this.playerWonPiles = new Dictionary<string, List<Card[]>>()
        {
            { "1", new List<Card[]>() },
            { "2", new List<Card[]>() },
            { "3", new List<Card[]>() },
            { "4", new List<Card[]>() },
        };

        this.playerPoints = new Dictionary<string, int[]>()
        {
            { "1", new int[numRounds] },
            { "2", new int[numRounds] },
            { "3", new int[numRounds] },
            { "4", new int[numRounds] },
        };

        // Set the initial state to the player.
        this.gameStateContext.SetState(playerState);

        var everythingRound = new EverythingRound(this.roundContext);
        var nothingRound = new NothingRound(this.roundContext, everythingRound);
        var pilesRound = new PilesRound(this.roundContext, nothingRound);
        var kingOfHeartsRound = new KingOfHeartsRound(this.roundContext, pilesRound);
        var queensRound = new QueensRound(this.roundContext, kingOfHeartsRound);
        var heartsRound = new HeartsRound(this.roundContext, queensRound);
        this.roundContext.SetState(heartsRound);

        // Listen for events when cards are being played.
        Card.onPlayed += this.OnCardPlayed;

        this.gameStateContext.Start();
    }
}
