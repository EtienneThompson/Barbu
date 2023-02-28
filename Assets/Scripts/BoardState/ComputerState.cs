using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerState : GameState
{
    private GameStateContext context;
    private GameState nextState;
    private Card[] hand;
    public int playerId;
    private StateMachine stateMachine;

    public ComputerState(GameStateContext context, int id, Card[] hand)
    {
        this.Initialize(context, hand, id);
    }

    public ComputerState(GameStateContext context, GameState next, int id, Card[] hand)
    {
        this.Initialize(context, hand, id);
        this.nextState = next;
    }

    public void Start()
    {
        if (!stateMachine.IsCardPlayable())
        {
            Debug.Log("Can't play a card.");
        }

        // Implement the logic for making a move here.
        foreach (var card in this.hand)
        {
            if (card.state == Card.CardState.Waiting)
            {
                card.PlayCard();
                break;
            }
        }
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

    private void Initialize(GameStateContext context, Card[] hand, int id)
    {
        this.context = context;
        this.hand = hand;
        this.playerId = id;
        this.stateMachine = new StateMachine();
    }
}
