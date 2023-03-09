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

    private RoundContext roundContext;
    private GameStateContext gameStateContext;
    private GameState[] players;
    private StateMachine stateMachine;

    private Dictionary<string, List<Card[]>> playerWonPiles;
    private Dictionary<string, int> playerPoints;

    public RoundManager(Hand[] hands)
    {
        this.roundContext = new RoundContext();
        this.gameStateContext = new GameStateContext();
        this.players = new GameState[4];
        this.stateMachine = new StateMachine();

        // Initialize general gameplay loop.
        var playerState = new PlayerState(this.gameStateContext, "1");
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

        this.playerPoints = new Dictionary<string, int>()
        {
            { "1", 0 },
            { "2", 0 },
            { "3", 0 },
            { "4", 0 },
        };

        // Set the initial state to the player.
        this.gameStateContext.SetState(playerState);

        var heartsRound = new HeartsRound(this.roundContext);
        this.roundContext.SetState(heartsRound);

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
            this.startingSuit = card.suit;
            GameObject startingSuitLabelObject = GameObject.Find("StartingSuitLabel");
            TextMeshProUGUI startingSuitLabel = startingSuitLabelObject.GetComponent<TextMeshProUGUI>();
            startingSuitLabel.text = "Starting suit: " + this.startingSuit;
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
        var playerId = this.currentPile[highestCardIndex].playerId;
        var player = this.GetPlayerFromId(playerId);
        this.gameStateContext.SetState(player);

        var copiedPile = (Card[])this.currentPile.Clone();
        this.playerWonPiles[playerId].Add(copiedPile);
        this.playerPoints[playerId] += this.roundContext.CalculatePointsInPile(this.currentPile);

        this.UpdateUiLabels();

        if (this.roundContext.IsRoundOver(this.playerPoints))
        {
            Debug.Log("ROUND OVER!!!");
        }

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

    private void UpdateUiLabels()
    {
        GameObject player1WonPilesObject = GameObject.Find("Player1WonPiles");
        TextMeshProUGUI player1WonPiles = player1WonPilesObject.GetComponent<TextMeshProUGUI>();
        player1WonPiles.text = "Player 1 Won Piles: " + this.playerWonPiles["1"].Count;

        GameObject player1PointsObject = GameObject.Find("Player1Points");
        TextMeshProUGUI player1Points = player1PointsObject.GetComponent<TextMeshProUGUI>();
        player1Points.text = "Player 1 Points: " + this.playerPoints["1"];

        GameObject player2WonPilesObject = GameObject.Find("Player2WonPiles");
        TextMeshProUGUI player2WonPiles = player2WonPilesObject.GetComponent<TextMeshProUGUI>();
        player2WonPiles.text = "Player 2 Won Piles: " + this.playerWonPiles["2"].Count;

        GameObject player2PointsObject = GameObject.Find("Player2Points");
        TextMeshProUGUI player2Points = player2PointsObject.GetComponent<TextMeshProUGUI>();
        player2Points.text = "Player 2 Points: " + this.playerPoints["2"];

        GameObject player3WonPilesObject = GameObject.Find("Player3WonPiles");
        TextMeshProUGUI player3WonPiles = player3WonPilesObject.GetComponent<TextMeshProUGUI>();
        player3WonPiles.text = "Player 3 Won Piles: " + this.playerWonPiles["3"].Count;

        GameObject player3PointsObject = GameObject.Find("Player3Points");
        TextMeshProUGUI player3Points = player3PointsObject.GetComponent<TextMeshProUGUI>();
        player3Points.text = "Player 3 Points: " + this.playerPoints["3"];

        GameObject player4WonPilesObject = GameObject.Find("Player4WonPiles");
        TextMeshProUGUI player4WonPiles = player4WonPilesObject.GetComponent<TextMeshProUGUI>();
        player4WonPiles.text = "Player 4 Won Piles: " + this.playerWonPiles["4"].Count;

        GameObject player4PointsObject = GameObject.Find("Player4Points");
        TextMeshProUGUI player4Points = player4PointsObject.GetComponent<TextMeshProUGUI>();
        player4Points.text = "Player 4 Points: " + this.playerPoints["4"];
    }
}
