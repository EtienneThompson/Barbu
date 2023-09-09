using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoundManager : IRoundManager, IEventListener
{
    protected Card[] currentPile = new Card[Constants.CardsPerPile];
    protected int pilesPlayed = 0;
    protected string roundStartingPlayerId = Constants.PlayerIds.Player1;
    protected int currentRound = 0;
    protected int totalRounds;
    protected Card highestCard;

    protected RoundContext roundContext;
    protected GameStateContext gameStateContext;
    protected GameState[] players;
    protected StateMachine stateMachine;
    protected ScoreMenu scoreMenu;
    protected GameBoard gameBoard;
    protected AdvertisementController advertisementController;
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
        this.gameStateContext = new GameStateContext(this.roundContext);
        this.players = new GameState[4];
        this.stateMachine = new StateMachine();
        this.stateMachine.SetStartingSuit(string.Empty);
        this.scoreMenu = scoreMenu;
        this.gameBoard = gameBoard;
        this.inGamePointsController = inGamePointsController;
        this.totalRounds = totalRounds;
        this.highestCard = null;
        this.advertisementController = this.gameBoard.GetComponent<AdvertisementController>();
        this.advertisementController.RequestToShowInterstitial();

        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, Constants.PlayerIds.Player1, hands[0]);
        var computerState3 = ComputerStateFactory.GetComputerStateFromSettings(this.gameStateContext, Constants.PlayerIds.Player4, hands[3], playerState);
        var computerState2 = ComputerStateFactory.GetComputerStateFromSettings(this.gameStateContext, Constants.PlayerIds.Player3, hands[2], computerState3);
        var computerState1 = ComputerStateFactory.GetComputerStateFromSettings(this.gameStateContext, Constants.PlayerIds.Player2, hands[1], computerState2);
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
        this.Setup();
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
        this.inGamePointsController.ResetRoundName();
    }

    /// <summary>
    /// Handles starting the round
    /// </summary>
    public void StartRound()
    {
        Debug.Log("StartRound");
        this.stateMachine.SetCardPlayable(true);
        this.gameStateContext.Start();
        this.inGamePointsController.SetRoundName(this.roundContext.CurrentName());
    }

    /// <summary>
    /// Handles cleaning up after a round finishes.
    /// </summary>
    public void CleanupRound()
    {
        Debug.Log("CleanupRound");
        this.stateMachine.SetCardPlayable(false);
        this.gameBoard.CleanupRound();
        this.inGamePointsController.ResetRoundName();
        if (this.currentRound + 1 == this.totalRounds)
        {
            Debug.Log("Marking game as finished");
            this.MarkGameAsFinished();
            Statistics.GetGamesFinished();

            var winningPlayer = this.GetWinningPlayerId();
            if (winningPlayer.Equals(Constants.PlayerIds.Player1))
            {
                Debug.Log("Marking game as won");
                this.MarkGameAsWon();
                Statistics.GetGamesWon();
            }

            this.advertisementController.ShowInterstitialAd();
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

    public void Setup()
    {
        // Listen for events when cards are being played.
        EventsController.playCard+= this.OnCardPlayed;
        EventsController.endRound += this.CleanupRound;
        EventsController.roundAnimationOver += this.StartRound;
        EventsController.endPileResolution += this.OnPileResolved;
    }

    public void Destroy()
    {
        // Deregister event listeners when this round is no longer applicable.
        EventsController.playCard -= this.OnCardPlayed;
        EventsController.endRound -= this.CleanupRound;
        EventsController.roundAnimationOver -= this.StartRound;
        EventsController.endPileResolution -= this.OnPileResolved;

        // Clean up any dependency listeners.
        this.gameStateContext.Destroy();
    }

    protected void OnCardPlayed(Card card)
    {
        this.stateMachine.SetCardPlayable(false);
        this.gameStateContext.CleanUp();

        if (this.stateMachine.NumCardsPlayed() == 1)
        {
            this.stateMachine.SetStartingSuit(card.suit);
        }

        this.currentPile[this.stateMachine.NumCardsPlayed() - 1] = card;

        if (highestCard != null)
        {
            highestCard.RemoveHighlight();
        }

        var highestCardIndex = this.FindHighestCardIndex();
        this.highestCard = this.currentPile[highestCardIndex];
        this.highestCard.Highlight(new Color(0.0f, 0.0f, 255.0f, 1.0f));

        if (this.stateMachine.NumCardsPlayed() == Constants.CardsPerPile) {
            this.ResolvePile();
            return;
        }

        this.stateMachine.SetCardPlayable(true);
        this.gameStateContext.Next();
    }

    protected void OnPileResolved()
    {
        this.stateMachine.SetCardPlayable(true);
        // Start the new state so that if the player is a computer they will make a move.
        this.gameStateContext.Start();
    }

    protected virtual void MarkGameAsFinished()
    {
        throw new Exception("This method must be overridden.");
    }

    protected virtual void MarkGameAsWon()
    {
        throw new Exception("This method must be overridden.");
    }

    private void ResolvePile()
    {
        this.pilesPlayed++;
        var highestCardIndex = this.FindHighestCardIndex();

        // Determine which player's card was the highest one played.
        var playerId = this.currentPile[highestCardIndex].playerId;
        var player = this.GetPlayerFromId(playerId);
        this.gameStateContext.SetState(player);

        var copiedPile = (Card[])this.currentPile.Clone();
        this.playerWonPiles[playerId].Add(copiedPile);
        var pilePoints = this.roundContext.CalculatePointsInPile(this.currentPile);
        this.playerPoints[playerId][this.currentRound] += pilePoints;

        this.inGamePointsController.UpdatePlayerPoints(playerId, pilePoints);

        for (int i = 0; i < this.stateMachine.NumCardsPlayed(); i++)
        {
            this.currentPile[i].GetComponent<Card>().StartPileResolution(playerId);
            this.currentPile[i] = null;
        }

        this.stateMachine.ResetNumCardsPlayed();
        this.highestCard = null;
        this.stateMachine.SetStartingSuit(string.Empty);

        if (this.roundContext.IsRoundOver(this.currentRound, this.playerPoints, this.pilesPlayed))
        {
            this.pilesPlayed = 0;
            this.scoreMenu.UpdateScores(this.currentRound, this.playerPoints);
        }
    }

    private string GetWinningPlayerId()
    {
        Dictionary<string, int> playerPointsSum = new Dictionary<string, int>
        {
            [Constants.PlayerIds.Player1] = this.playerPoints[Constants.PlayerIds.Player1].Sum(),
            [Constants.PlayerIds.Player2] = this.playerPoints[Constants.PlayerIds.Player2].Sum(),
            [Constants.PlayerIds.Player3] = this.playerPoints[Constants.PlayerIds.Player3].Sum(),
            [Constants.PlayerIds.Player4] = this.playerPoints[Constants.PlayerIds.Player4].Sum(),
        };

        return playerPointsSum.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
    }

    private int FindHighestCardIndex()
    {
        var highestCardIndex = 0;
        for (int i = 0; i < this.currentPile.Length; i++)
        {
            if (this.currentPile[i] == null)
            {
                continue;
            }

            if (this.currentPile[i].suit == this.stateMachine.GetStartingSuit() &&
                this.currentPile[i].rank > this.currentPile[highestCardIndex].rank)
            {
                highestCardIndex = i;
            }
        }

        return highestCardIndex;
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
