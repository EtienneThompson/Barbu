using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager
{
    private string startingSuit;
    private const int cardsPerPile = 4;
    private Card[] currentPile = new Card[cardsPerPile];
    private int numCardsInPile = 0;

    private RoundContext roundContext;
    private GameStateContext gameStateContext;
    private PlayerContext[] playerContexts;
    private StateMachine stateMachine;

    public RoundManager(Hand[] hands)
    {
        this.roundContext = new RoundContext();
        this.gameStateContext = new GameStateContext();
        this.playerContexts = new PlayerContext[4];
        this.stateMachine = new StateMachine();

        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, 1);
        var computerState3 = new ComputerState(this.gameStateContext, playerState, 4, hands[3]);
        var computerState2 = new ComputerState(this.gameStateContext, computerState3, 3, hands[2]);
        var computerState1 = new ComputerState(this.gameStateContext, computerState2, 2, hands[1]);
        playerState.SetNextState(computerState1);

        this.playerContexts[0] = new PlayerContext(1, playerState);
        this.playerContexts[1] = new PlayerContext(2, computerState1);
        this.playerContexts[2] = new PlayerContext(3, computerState2);
        this.playerContexts[3] = new PlayerContext(4, computerState3);

        // Set the initial state to the player.
        this.gameStateContext.SetState(playerState);

        var heartsRound = new HeartsRound(this.roundContext);

        // Listen for events when cards are being played.
        Card.onPlayed += this.OnCardPlayed;

        this.gameStateContext.Start();
    }

    public void NextRound()
    {
        this.roundContext.Next();
    }

    public void SetStartingPlayer(GameState player)
    {
        this.gameStateContext.SetState(player);
    }

    public GameState GetPlayerFromId(int id)
    {
        foreach (var state in this.playerContexts)
        {
            if (id == state.GetId())
            {
                return state.GetGameState();
            }
        }

        return null;
    }

    private void OnCardPlayed(Card card)
    {
        this.stateMachine.SetCardPlayable(false);

        if (this.numCardsInPile == 0)
        {
            this.startingSuit = card.suit;
        }
        
        this.currentPile[this.numCardsInPile] = card;
        this.numCardsInPile++;

        if (this.numCardsInPile == cardsPerPile) {
            this.ResolvePile();
        }

        this.stateMachine.SetCardPlayable(true);

        if (this.numCardsInPile == 0)
        {
            // Start the new state so that if the player is a computer they will make a move.
            this.gameStateContext.Start();
        }
        else
        {
            // If we just resolved a pile and therefore have no cards, then we
            // don't want to move past the starting player state.
            this.gameStateContext.Next();
        }
    }

    private void ResolvePile()
    {
        var highestCardIndex = 0;
        for (int i = 0; i < this.numCardsInPile; i++)
        {
            if (this.currentPile[i].suit == this.startingSuit &&
                this.currentPile[i].rank > this.currentPile[highestCardIndex].rank)
            {
                highestCardIndex = i;
            }
        }

        // Determine which player's card was the highest one played.
        var player = this.GetPlayerFromId(this.currentPile[highestCardIndex].playerId);
        this.gameStateContext.SetState(player);

        var copiedPile = (Card[])this.currentPile.Clone();

        // Hide the cards in the UI.
        for (int i = 0; i < this.numCardsInPile; i++)
        {
            this.currentPile[i].gameObject.SetActive(false);
            this.currentPile[i].GetComponent<Renderer>().enabled = false;
            this.currentPile[i] = null;
        }

        this.numCardsInPile = 0;
        this.stateMachine.ResetNumCardsPlayed();
    }
}
