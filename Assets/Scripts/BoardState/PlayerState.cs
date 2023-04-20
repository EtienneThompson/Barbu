using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerState : GameState
{
    public string playerId;
    private GameStateContext context;
    private GameState nextState;
    private Hand hand;
    private StateMachine stateMachine;

    public PlayerState(GameStateContext context, string id, Hand hand)
    {
        this.Initialize(context, id, hand);
    }

    public PlayerState(GameStateContext context, GameState next, string id, Hand hand)
    {
        this.Initialize(context, id, hand);
        this.nextState = next;
    }

    public string PlayerId => this.playerId;

    public void Start()
    {
        // Determine if the player must play a card in the starting suite based
        // on if they are the first player in a round and the cards in their hand.
        var startingPlayer = string.IsNullOrEmpty(this.stateMachine.GetStartingSuit());
        this.stateMachine.SetPlayerMustPlayStartingSuit(startingPlayer, hand);

        // Highlight all cards that are of the starting suit.
        var cardsInSuit = this.hand.CardsInSuit(this.stateMachine.GetStartingSuit());
        if (cardsInSuit.Count == 0)
        {
            cardsInSuit = this.hand.GetAvailableCards();
        }
        foreach (var card in cardsInSuit)
        {
            card.Highlight();
        }

        GameObject player1WonPilesObject = GameObject.Find("MustPlayStartingSuit");
        TextMeshProUGUI player1WonPiles = player1WonPilesObject.GetComponent<TextMeshProUGUI>();
        player1WonPiles.text = "Must Play Starting Suit: " + this.stateMachine.MustPlayCardInStartingSuit();
    }

    public void CleanUp()
    {
        // When the player has finished and we should move on to the next state,
        // remove all highlights.
        foreach (var card in this.hand.GetHand())
        {
            card.RemoveHighlight();
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

    public Hand GetHand()
    {
        return this.hand;
    }

    public void SetHand(Hand newHand)
    {
        this.hand = newHand;
    }

    private void Initialize(GameStateContext context, string id, Hand hand)
    {
        this.context = context;
        this.playerId = id;
        this.hand = hand;
        this.stateMachine = new StateMachine();
    }
}
