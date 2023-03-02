using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager
{
    private RoundContext roundContext;
    private GameStateContext gameStateContext;

    private Dictionary<int, GameState> playerMap;

    public RoundManager(Hand[] hands)
    {
        this.roundContext = new RoundContext();
        this.gameStateContext = new GameStateContext();

        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, 1);
        var computerState3 = new ComputerState(this.gameStateContext, playerState, 4, hands[3]);
        var computerState2 = new ComputerState(this.gameStateContext, computerState3, 3, hands[2]);
        var computerState1 = new ComputerState(this.gameStateContext, computerState2, 2, hands[1]);
        playerState.SetNextState(computerState1);

        this.playerMap = new Dictionary<int, GameState>
        {
            {1, playerState},
            {2, computerState1},
            {3, computerState2},
            {4, computerState3}
        };

        // Set the initial state to the player.
        this.gameStateContext.SetState(playerState);
    }

    public void NextGameState()
    {
        this.gameStateContext.Next();
    }

    public void StartGameState()
    {
        this.gameStateContext.Start();
    }

    public void NextRound()
    {
        this.roundContext.Next();
    }

    public void SetStartingSuit(string suit)
    {
        this.gameStateContext.SetStartingSuit(suit);
    }

    public string GetStartingSuit()
    {
        return this.gameStateContext.GetStartingSuit();
    }

    public void SetStartingPlayer(GameState player)
    {
        this.gameStateContext.SetState(player);
    }

    public GameState GetPlayerFromId(int id)
    {
        if (!this.playerMap.TryGetValue(id, out var player))
        {
            return null;
        }

        return player;
    }
}
