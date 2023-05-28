using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseRoundManager : IRoundManager
{
    protected Card[] currentPile = new Card[Constants.CardsPerPile];
    protected int pilesPlayed = 0;
    protected string roundStartingPlayerId = Constants.PlayerIds.Player1;
    protected int currentRound = 0;
    protected int totalRounds;

    protected RoundContext roundContext;
    protected GameStateContext gameStateContext;
    protected GameState[] players;
    protected StateMachine stateMachine;
    protected ScoreMenu scoreMenu;
    protected GameBoard gameBoard;
    protected InGamePointsController inGamePointsController;
    protected Dictionary<string, List<Card[]>> playerWonPiles;
    protected Dictionary<string, int[]>playerPoints;

    public BaseRoundManager(
        int totalRounds,
        GameBoard gameBoard,
        ScoreMenu scoreMenu,
        InGamePointsController inGamePointsController,
        Hand[] hands)
    {
        this.roundContext = new RoundContext();
        this.gameStateContext = new GameStateContext();
        this.players = new GameState[4];
        this.stateMachine = new StateMachine();
        this.stateMachine.SetStartingSuit(string.Empty);
        this.scoreMenu = scoreMenu;
        this.gameBoard = gameBoard;
        this.inGamePointsController = inGamePointsController;
        this.totalRounds = totalRounds;

        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, Constants.PlayerIds.Player1, hands[0]);
        var computerState3 = new ComputerState(this.gameStateContext, playerState, Constants.PlayerIds.Player4, hands[3]);
        var computerState2 = new ComputerState(this.gameStateContext, computerState3, Constants.PlayerIds.Player3, hands[2]);
        var computerState1 = new ComputerState(this.gameStateContext, computerState2, Constants.PlayerIds.Player2, hands[1]);
        playerState.SetNextState(computerState1);

        this.players[0] = playerState;
        this.players[1] = computerState1;
        this.players[2] = computerState2;
        this.players[3] = computerState3;

        this.playerWonPiles = new Dictionary<string, List<Card[]>>()
        {
            { Constants.PlayerIds.Player1, new List<Card[]>() },
            { Constants.PlayerIds.Player2, new List<Card[]>() },
            { Constants.PlayerIds.Player3, new List<Card[]>() },
            { Constants.PlayerIds.Player4, new List<Card[]>() },
        };

        this.playerPoints = new Dictionary<string, int[]>()
        {
            { Constants.PlayerIds.Player1, new int[this.totalRounds] },
            { Constants.PlayerIds.Player2, new int[this.totalRounds] },
            { Constants.PlayerIds.Player3, new int[this.totalRounds] },
            { Constants.PlayerIds.Player4, new int[this.totalRounds] },
        };

        // Set the initial state to the player.
        this.gameStateContext.SetState(playerState);

        // Listen for events when cards are being played.
        Card.onPlayed += this.OnCardPlayed;
        ScoreMenu.onRoundOver += this.CleanupRound;
        RoundOverlayController.finishedAnimation += this.StartRound;
    }

    /// <summary>
    /// Handles any pre-processing needed before each round.
    /// </summary>
    public void PreRound()
    {
        Debug.Log("PreRound");
        GameObject roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
        var controller = roundOverlay.GetComponent<RoundOverlayController>();
        controller.DisplayRound(this.roundContext.CurrentName());
    }

    /// <summary>
    /// Handles starting the round
    /// </summary>
    public void StartRound()
    {
        Debug.Log("StartRound");
        this.stateMachine.SetCardPlayable(true);
        this.gameStateContext.Start();
    }

    /// <summary>
    /// Handles cleaning up after a round finishes.
    /// </summary>
    public void CleanupRound()
    {
        Debug.Log("CleanupRound");
        this.stateMachine.SetCardPlayable(false);
        this.gameBoard.CleanupRound();
        if (this.currentRound + 1 == this.totalRounds)
        {
            AdvertisementController advertisementController = this.gameBoard.GetComponent<AdvertisementController>();
            advertisementController.RequestToShowInterstitial();
        }
        else
        {
            Debug.Log("ROUND OVER!!!");
            this.gameBoard.SetupRound();
            this.inGamePointsController.ResetPoints();
        }
    }

    /// <summary>
    /// Moves to the next round.
    /// </summary>
    public void NextRound(Hand[] hands)
    {
        Debug.Log("NextRound");
        this.players[0].SetHand(hands[0]);
        this.players[1].SetHand(hands[1]);
        this.players[2].SetHand(hands[2]);
        this.players[3].SetHand(hands[3]);
        this.currentRound++;
        this.roundContext.Next();
        var currentStartingPlayer = Int32.Parse(this.roundStartingPlayerId);
        Debug.Log("Last round starting player: " + this.roundStartingPlayerId);
        var newStartingPlayer = (currentStartingPlayer % 4) + 1;
        Debug.Log("New starting player id: " + newStartingPlayer);
        this.roundStartingPlayerId = newStartingPlayer.ToString();
        var player = this.GetPlayerFromId(this.roundStartingPlayerId);
        Debug.Log("Next round starting player: " + player);
        this.gameStateContext.SetState(player);
        this.PreRound();
    }

    public void Destroy()
    {
        // Deregister event listeners when this round is no longer applicable.
        Card.onPlayed -= this.OnCardPlayed;
        ScoreMenu.onRoundOver -= this.CleanupRound;
    }

    protected void OnCardPlayed(Card card)
    {
        this.stateMachine.SetCardPlayable(false);
        this.gameStateContext.CleanUp();

        if (this.stateMachine.NumCardsPlayed() == 1)
        {
            this.stateMachine.SetStartingSuit(card.suit);
            GameObject startingSuitLabelObject = GameObject.Find("StartingSuitLabel");
            TextMeshProUGUI startingSuitLabel = startingSuitLabelObject.GetComponent<TextMeshProUGUI>();
            startingSuitLabel.text = "Starting Suit: " + card.suit;
            card.Highlight(Color.green);
        }
        
        this.currentPile[this.stateMachine.NumCardsPlayed() - 1] = card;

        if (this.stateMachine.NumCardsPlayed() == Constants.CardsPerPile) {
            if (this.ResolvePile())
            {
                return;
            }
        }

        this.stateMachine.SetCardPlayable(true);

        if (this.stateMachine.NumCardsPlayed() == 0)
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

    private bool ResolvePile()
    {
        this.pilesPlayed++;
        var highestCardIndex = 0;
        for (int i = 0; i < Constants.CardsPerPile; i++)
        {
            if (this.currentPile[i].suit == this.stateMachine.GetStartingSuit() &&
                this.currentPile[i].rank > this.currentPile[highestCardIndex].rank)
            {
                highestCardIndex = i;
            }
        }

        // Determine which player's card was the highest one played.
        var playerId = this.currentPile[highestCardIndex].playerId;
        var player = this.GetPlayerFromId(playerId);
        this.gameStateContext.SetState(player);

        var copiedPile = (Card[])this.currentPile.Clone();
        this.playerWonPiles[playerId].Add(copiedPile);
        var pilePoints = this.roundContext.CalculatePointsInPile(this.currentPile);
        this.playerPoints[playerId][this.currentRound] += pilePoints;

        this.inGamePointsController.UpdatePlayerPoints(playerId, pilePoints);

        // Hide the cards in the UI.
        for (int i = 0; i < this.stateMachine.NumCardsPlayed(); i++)
        {
            this.currentPile[i].gameObject.SetActive(false);
            this.currentPile[i].GetComponent<Renderer>().enabled = false;
            this.currentPile[i] = null;
        }

        this.stateMachine.ResetNumCardsPlayed();
        this.stateMachine.SetStartingSuit(string.Empty);

        if (this.roundContext.IsRoundOver(this.currentRound, this.playerPoints, this.pilesPlayed))
        {
            this.pilesPlayed = 0;
            this.scoreMenu.UpdateScores(this.currentRound, this.playerPoints);
            return true;
        }

        return false;
    }

    private GameState GetPlayerFromId(string id)
    {
        foreach (var state in this.players)
        {
            if (id.Equals(state.PlayerId))
            {
                return state;
            }
        }

        return null;
    }
}
