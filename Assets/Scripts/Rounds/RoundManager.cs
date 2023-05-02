using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager
{
    private string startingSuit;
    private const int cardsPerPile = 4;
    private Card[] currentPile = new Card[cardsPerPile];
    private int numCardsInPile = 0;
    private string roundStartingPlayerId = "1";
    private int currentRound = 0;

    private RoundContext roundContext;
    private GameStateContext gameStateContext;
    private GameState[] players;
    private StateMachine stateMachine;
    private ScoreMenu scoreMenu;
    private int maxRounds;

    private Dictionary<string, List<Card[]>> playerWonPiles;
    private Dictionary<string, int[]> playerPoints;

    public RoundManager(int numRounds, ScoreMenu scoreMenu, Hand[] hands)
    {
        this.roundContext = new RoundContext();
        this.gameStateContext = new GameStateContext();
        this.players = new GameState[4];
        this.stateMachine = new StateMachine();
        this.stateMachine.SetStartingSuit(string.Empty);
        this.scoreMenu = scoreMenu;
        this.maxRounds = numRounds;

        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, "1", hands[0]);
        var computerState3 = new ComputerState(this.gameStateContext, playerState, "4", hands[3]);
        var computerState2 = new ComputerState(this.gameStateContext, computerState3, "3", hands[2]);
        var computerState1 = new ComputerState(this.gameStateContext, computerState2, "2", hands[1]);
        playerState.SetNextState(computerState1);

        this.players[0] = playerState;
        this.players[1] = computerState1;
        this.players[2] = computerState2;
        this.players[3] = computerState3;

        this.playerWonPiles = new Dictionary<string, List<Card[]>>()
        {
            { "1", new List<Card[]>() },
            { "2", new List<Card[]>() },
            { "3", new List<Card[]>() },
            { "4", new List<Card[]>() },
        };

        this.playerPoints = new Dictionary<string, int[]>()
        {
            { "1", new int[numRounds] },
            { "2", new int[numRounds] },
            { "3", new int[numRounds] },
            { "4", new int[numRounds] },
        };

        // Set the initial state to the player.
        this.gameStateContext.SetState(playerState);

        var everythingRound = new EverythingRound(this.roundContext);
        var nothingRound = new NothingRound(this.roundContext, everythingRound);
        var pilesRound = new PilesRound(this.roundContext, nothingRound);
        var kingOfHeartsRound = new KingOfHeartsRound(this.roundContext, pilesRound);
        var queensRound = new QueensRound(this.roundContext, kingOfHeartsRound);
        var heartsRound = new HeartsRound(this.roundContext, queensRound);
        this.roundContext.SetState(heartsRound);

        // Listen for events when cards are being played.
        Card.onPlayed += this.OnCardPlayed;

        this.gameStateContext.Start();
    }

    public void NextRound(Hand[] hands)
    {
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
        this.gameStateContext.Start();
    }

    public void SetStartingPlayer(GameState player)
    {
        this.gameStateContext.SetState(player);
    }

    public GameState GetPlayerFromId(string id)
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

    private void OnCardPlayed(Card card)
    {
        this.stateMachine.SetCardPlayable(false);

        if (this.numCardsInPile == 0)
        {
            this.stateMachine.SetStartingSuit(card.suit);
            GameObject startingSuitLabelObject = GameObject.Find("StartingSuitLabel");
            TextMeshProUGUI startingSuitLabel = startingSuitLabelObject.GetComponent<TextMeshProUGUI>();
            startingSuitLabel.text = "Starting Suit: " + card.suit;
        }
        
        this.currentPile[this.numCardsInPile] = card;
        this.numCardsInPile++;

        this.gameStateContext.CleanUp();

        if (this.numCardsInPile == cardsPerPile) {
            if (this.ResolvePile())
            {
                return;
            }
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

    private bool ResolvePile()
    {
        var highestCardIndex = 0;
        for (int i = 0; i < this.numCardsInPile; i++)
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
        this.playerPoints[playerId][this.currentRound] += this.roundContext.CalculatePointsInPile(this.currentPile);

        this.UpdateUiLabels();

        // Hide the cards in the UI.
        for (int i = 0; i < this.numCardsInPile; i++)
        {
            this.currentPile[i].gameObject.SetActive(false);
            this.currentPile[i].GetComponent<Renderer>().enabled = false;
            this.currentPile[i] = null;
        }

        this.numCardsInPile = 0;
        this.stateMachine.ResetNumCardsPlayed();
        this.stateMachine.SetStartingSuit(string.Empty);

        if (this.roundContext.IsRoundOver(this.currentRound, this.playerPoints))
        {
            this.scoreMenu.UpdateScores(this.currentRound, this.playerPoints);
            return true;
        }

        return false;
    }

    private void UpdateUiLabels()
    {
        GameObject player1WonPilesObject = GameObject.Find("Player1WonPiles");
        TextMeshProUGUI player1WonPiles = player1WonPilesObject.GetComponent<TextMeshProUGUI>();
        player1WonPiles.text = "Player 1 Won Piles: " + this.playerWonPiles["1"].Count;

        GameObject player1PointsObject = GameObject.Find("Player1Points");
        TextMeshProUGUI player1Points = player1PointsObject.GetComponent<TextMeshProUGUI>();
        player1Points.text = "Player 1 Points: " + this.playerPoints["1"][this.currentRound];

        GameObject player2WonPilesObject = GameObject.Find("Player2WonPiles");
        TextMeshProUGUI player2WonPiles = player2WonPilesObject.GetComponent<TextMeshProUGUI>();
        player2WonPiles.text = "Player 2 Won Piles: " + this.playerWonPiles["2"].Count;

        GameObject player2PointsObject = GameObject.Find("Player2Points");
        TextMeshProUGUI player2Points = player2PointsObject.GetComponent<TextMeshProUGUI>();
        player2Points.text = "Player 2 Points: " + this.playerPoints["2"][this.currentRound];

        GameObject player3WonPilesObject = GameObject.Find("Player3WonPiles");
        TextMeshProUGUI player3WonPiles = player3WonPilesObject.GetComponent<TextMeshProUGUI>();
        player3WonPiles.text = "Player 3 Won Piles: " + this.playerWonPiles["3"].Count;

        GameObject player3PointsObject = GameObject.Find("Player3Points");
        TextMeshProUGUI player3Points = player3PointsObject.GetComponent<TextMeshProUGUI>();
        player3Points.text = "Player 3 Points: " + this.playerPoints["3"][this.currentRound];

        GameObject player4WonPilesObject = GameObject.Find("Player4WonPiles");
        TextMeshProUGUI player4WonPiles = player4WonPilesObject.GetComponent<TextMeshProUGUI>();
        player4WonPiles.text = "Player 4 Won Piles: " + this.playerWonPiles["4"].Count;

        GameObject player4PointsObject = GameObject.Find("Player4Points");
        TextMeshProUGUI player4Points = player4PointsObject.GetComponent<TextMeshProUGUI>();
        player4Points.text = "Player 4 Points: " + this.playerPoints["4"][this.currentRound];
    }
}
