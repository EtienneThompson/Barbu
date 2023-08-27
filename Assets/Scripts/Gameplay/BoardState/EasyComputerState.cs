using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyComputerState : GameState
{
    public EasyComputerState(GameStateContext context, string id, Hand hand)
    : base(context, hand, id)
    {
    }

    public EasyComputerState(GameStateContext context, GameState next, string id, Hand hand)
    : base(context, next, hand, id)
    {
    }

    public override void Start()
    {
        if (!this.stateMachine.IsCardPlayable())
        {
            throw new Exception("Computer can't make a move right now.");
        }

        this.stateMachine.SetCardPlayable(false);
        var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
        if (cardsInSuit.Count > 0)
        {
            foreach (var card in cardsInSuit)
            {
                if (card.state == Card.CardState.Waiting)
                {
                    card.PlayCard();
                    return;
                }
            }
        }
        else
        {
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
}
