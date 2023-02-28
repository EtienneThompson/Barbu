using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager
{
    private RoundContext roundContext;
    private GameStateContext gameStateContext;

    public RoundManager(Card[,] hands)
    {
        this.roundContext = new RoundContext();
        this.gameStateContext = new GameStateContext();

        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, 1);
        var computerState3 = new ComputerState(this.gameStateContext, playerState, 4, GetRow(hands, 3));
        var computerState2 = new ComputerState(this.gameStateContext, computerState3, 3, GetRow(hands, 2));
        var computerState1 = new ComputerState(this.gameStateContext, computerState2, 2, GetRow(hands, 1));
        playerState.SetNextState(computerState1);

        // Set the initial state to the player.
        this.gameStateContext.SetState(playerState);
    }

    public void NextGameState()
    {
        this.gameStateContext.Next();
    }

    public void NextRound()
    {
        this.roundContext.Next();
    }

    private T[] GetRow<T>(T[,] matrix, int rowNumber)
    {
        return Enumerable
            .Range(0, matrix.GetLength(1))
            .Select(x => matrix[rowNumber, x])
            .ToArray();
    }
}
