using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerState : GameState
{
    private GameStateContext context;
    private GameState nextState;
    private Hand hand;
    public string playerId;
    private StateMachine stateMachine;

    public ComputerState(GameStateContext context, string id, Hand hand)
    {
        this.Initialize(context, hand, id);
    }

    public ComputerState(GameStateContext context, GameState next, string id, Hand hand)
    {
        this.Initialize(context, hand, id);
        this.nextState = next;
    }

    public string PlayerId => this.playerId;

    public void Start()
    {
        if (!stateMachine.IsCardPlayable())
        {
            throw new Exception("Computer can't make a move right now.");
        }

        var cardsInSuit = this.hand.CardsInSuit(this.context.GetStartingSuit());
        if (cardsInSuit.Count > 0)
        {
            foreach (var card in this.hand.GetHand())
            {
                if (card.state == Card.CardState.Waiting && card.suit == this.context.GetStartingSuit())
                {
                    card.PlayCard();
                    return;
                }
            }
        }
        else
        {
            // If no cards in hand of the same suit, then pick a random one.
            foreach (var card in this.hand.GetHand())
            {
                if (card.state == Card.CardState.Waiting)
                {
                    card.PlayCard();
                    return;
                }
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

    private void Initialize(GameStateContext context, Hand hand, string id)
    {
        this.context = context;
        this.hand = hand;
        this.playerId = id;
        this.stateMachine = new StateMachine();
    }
}
