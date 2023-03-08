using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : GameState
{
    public int playerId;
    private GameStateContext context;
    private GameState nextState;

    public PlayerState(GameStateContext context, int id)
    {
        this.Initialize(context, id);
    }

    public PlayerState(GameStateContext context, GameState next, int id)
    {
        this.Initialize(context, id);
        this.nextState = next;
    }

    public int PlayerId => this.playerId;

    public void Start()
    {
        // Don't do anything as the player state waits for the player to make
        // a move.
    }

    public void GoNext()
    {
        if (this.nextState == null)
        {
            throw new Exception("No next state set.");
        }

        this.context.SetState(this.nextState);
    }

    public void SetNextState(GameState next)
    {
        this.nextState = next;
    }

    public int GetId()
    {
        return this.playerId;
    }

    private void Initialize(GameStateContext context, int id)
    {
        this.context = context;
        this.playerId = id;
    }

    private void GoNext(Card card)
    {
        this.GoNext();
    }
}
