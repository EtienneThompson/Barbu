namespace Barbu.Gameplay.Rounds.Managers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Gameplay.BoardState;
    using Barbu.Interfaces;
    using Barbu.Interfaces.Rounds;
    using Barbu.Models;
    using Barbu.UI.Controllers;
    using UnityEngine;
    using Barbu.Interfaces.Core;
    using Barbu.Core.Workflows.PlayTrickWorkflow;

    public class BaseRoundManager : IRoundManager, IEventListener
    {
        protected Pile currentPile;
        protected int pilesPlayed = 0;
        protected string roundStartingPlayerId = Constants.PlayerIds.Player1;
        protected int currentRound = 0;
        protected int totalRounds;
        protected Card highestCard;

        protected RoundContext roundContext;
        protected GameStateContext gameStateContext;
        protected PlayTrickWorkflow playTrickWorkflow;
        protected GameState[] players;
        protected Hand[] hands;
        protected StateMachine stateMachine;
        protected EventsController eventsController;
        protected ITelemetryService telemetryService;
        protected ScoreMenu scoreMenu;
        protected GameBoard gameBoard;
        protected AdvertisementController advertisementController;
        protected InGamePointsController inGamePointsController;
        protected Dictionary<string, List<Pile>> playerWonPiles;
        protected Dictionary<string, int[]> playerPoints;

        public BaseRoundManager(
            int totalRounds,
            GameBoard gameBoard,
            ScoreMenu scoreMenu,
            InGamePointsController inGamePointsController,
            Hand[] hands)
        {
            this.roundContext = new RoundContext();
            this.gameStateContext = new GameStateContext(this.roundContext);
            this.hands = hands;
            this.players = new GameState[4];
            this.stateMachine = new StateMachine();
            this.eventsController = EventsController.GetInstance();
            this.stateMachine.SetStartingSuit(string.Empty);
            this.telemetryService = TelemetryService.GetInstance();
            this.currentPile = new Pile();
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

            this.playerWonPiles = new Dictionary<string, List<Pile>>()
        {
            { Constants.PlayerIds.Player1, new List<Pile>() },
            { Constants.PlayerIds.Player2, new List<Pile>() },
            { Constants.PlayerIds.Player3, new List<Pile>() },
            { Constants.PlayerIds.Player4, new List<Pile>() },
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
            this.telemetryService.LogInfo("PreRound");
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
            this.telemetryService.LogInfo("StartRound");
            this.stateMachine.ResetNumCardsPlayed();

            if (this.roundContext.IsRoundOver(this.currentRound, this.playerPoints, this.pilesPlayed))
            {
                this.pilesPlayed = 0;
                this.scoreMenu.UpdateScores(this.currentRound, this.playerPoints);
                return;
            }

            if (this.playTrickWorkflow != null)
            {
                this.telemetryService.LogInfo($"[BaseRoundManager] Setting winning player: {this.playTrickWorkflow.GetWinningPlayerId()}");
                this.roundStartingPlayerId = this.playTrickWorkflow.GetWinningPlayerId();
            }

            this.playTrickWorkflow = new PlayTrickWorkflow(
                this.roundContext,
                this.inGamePointsController,
                this.playerPoints,
                this.hands,
                Int32.Parse(this.roundStartingPlayerId) - 1,
                this.currentRound);
            this.playTrickWorkflow.StartAsync();
            this.pilesPlayed++;
            // this.gameStateContext.Start();
            this.inGamePointsController.SetRoundName(this.roundContext.CurrentName());
        }

        /// <summary>
        /// Handles cleaning up after a round finishes.
        /// </summary>
        public void CleanupRound()
        {
            this.telemetryService.LogInfo("CleanupRound");
            this.stateMachine.SetCardPlayable(false);
            this.gameBoard.CleanupRound();
            this.inGamePointsController.ResetRoundName();
            if (this.currentRound + 1 == this.totalRounds)
            {
                this.telemetryService.LogInfo("Marking game as finished");
                this.CompleteGame();
            }
            else
            {
                this.telemetryService.LogInfo("ROUND OVER!!!");
                this.gameBoard.SetupRound();
                this.inGamePointsController.ResetPoints();
            }
        }

        /// <summary>
        /// Moves to the next round.
        /// </summary>
        public void NextRound(Hand[] hands)
        {
            this.telemetryService.LogInfo("NextRound");
            this.players[0].SetHand(hands[0]);
            this.players[1].SetHand(hands[1]);
            this.players[2].SetHand(hands[2]);
            this.players[3].SetHand(hands[3]);
            this.currentRound++;
            this.roundContext.Next();
            var currentStartingPlayer = Int32.Parse(this.roundStartingPlayerId);
            this.telemetryService.LogInfo("Last round starting player: " + this.roundStartingPlayerId);
            var newStartingPlayer = (currentStartingPlayer % 4) + 1;
            this.telemetryService.LogInfo("New starting player id: " + newStartingPlayer);
            this.roundStartingPlayerId = newStartingPlayer.ToString();
            var player = this.GetPlayerFromId(this.roundStartingPlayerId);
            this.telemetryService.LogInfo("Next round starting player: " + player);
            this.gameStateContext.SetState(player);
            this.PreRound();
        }

        public void CompleteGame()
        {
            this.MarkGameAsFinished();
            this.telemetryService.LogInfo("Games finished: " + Statistics.GetGamesFinished());

            var winningPlayers = this.GetWinningPlayerIds();
            this.telemetryService.LogInfo("Winning players: " + winningPlayers.Length);
            if (winningPlayers.Where(id => id == Constants.PlayerIds.Player1).Any())
            {
                this.telemetryService.LogInfo("Marking game as won");
                this.MarkGameAsWon();
                this.telemetryService.LogInfo("Games won: " + Statistics.GetGamesWon());
            }

            GameObject roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
            var controller = roundOverlay.GetComponent<RoundOverlayController>();
            controller.DisplayWinner(winningPlayers);
        }

        public void Setup()
        {
            // Listen for events when cards are being played.
            // this.eventsController.Subscribe(EventNames.PlayCard, this.OnCardPlayed);
            this.eventsController.Subscribe(EventNames.RoundOver, this.CleanupRound);
            this.eventsController.Subscribe(EventNames.RoundAnimationFinished, this.StartRound);
            this.eventsController.Subscribe(EventNames.PileResolved, this.StartRound);
            this.eventsController.Subscribe(EventNames.WinnerAnimationFinished, this.OnWinnerDisplayed);
        }

        public void Destroy()
        {
            // Deregister event listeners when this round is no longer applicable.
            // this.eventsController.Unsubscribe(EventNames.PlayCard, this.OnCardPlayed);
            this.eventsController.Unsubscribe(EventNames.RoundOver, this.CleanupRound);
            this.eventsController.Unsubscribe(EventNames.RoundAnimationFinished, this.StartRound);
            this.eventsController.Unsubscribe(EventNames.PileResolved, this.StartRound);
            this.eventsController.Unsubscribe(EventNames.WinnerAnimationFinished, this.OnWinnerDisplayed);

            // Clean up any dependency listeners.
            this.gameStateContext.Destroy();
        }

        /*
        protected void OnCardPlayed(object payload)
        {
            var card = (Card)payload;
            this.stateMachine.SetCardPlayable(false);
            this.gameStateContext.CleanUp();

            if (this.stateMachine.NumCardsPlayed() == 1)
            {
                this.stateMachine.SetStartingSuit(card.suit);
            }

            this.currentPile.AddCardToPile(card);

            if (highestCard != null)
            {
                highestCard.RemoveHighlight();
            }

            this.highestCard = this.currentPile.GetHighestCard();
            this.highestCard.Highlight(new Color(0.0f, 0.0f, 255.0f, 1.0f));
            this.stateMachine.SetHighestRank(this.highestCard.rank);

            if (this.stateMachine.NumCardsPlayed() == Constants.CardsPerPile)
            {
                this.ResolvePile();
                return;
            }

            this.stateMachine.SetCardPlayable(true);
            this.gameStateContext.Next();
        }
        */

        protected void OnPileResolved()
        {
            this.stateMachine.SetCardPlayable(true);
            // Start the new state so that if the player is a computer they will make a move.
            this.gameStateContext.Start();
        }

        protected void OnWinnerDisplayed()
        {
            this.advertisementController.ShowInterstitialAd();
        }

        protected virtual void MarkGameAsFinished()
        {
            throw new Exception("This method must be overridden.");
        }

        protected virtual void MarkGameAsWon()
        {
            throw new Exception("This method must be overridden.");
        }

        /*
        private void ResolvePile()
        {
            this.pilesPlayed++;
            var highestCard = this.currentPile.GetHighestCard();

            // Determine which player's card was the highest one played.
            var playerId = highestCard.playerId;
            var player = this.GetPlayerFromId(playerId);
            this.gameStateContext.SetState(player);

            this.playerWonPiles[playerId].Add(this.currentPile);
            var pilePoints = this.roundContext.CalculatePointsInPile(this.currentPile);
            this.playerPoints[playerId][this.currentRound] += pilePoints;

            this.inGamePointsController.UpdatePlayerPoints(playerId, pilePoints);

            this.currentPile.StartPileResolution(playerId);
            this.currentPile = new Pile();

            this.stateMachine.ResetNumCardsPlayed();
            this.stateMachine.SetHighestRank(0);
            this.highestCard = null;
            this.stateMachine.SetStartingSuit(string.Empty);

            if (this.roundContext.IsRoundOver(this.currentRound, this.playerPoints, this.pilesPlayed))
            {
                this.pilesPlayed = 0;
                this.scoreMenu.UpdateScores(this.currentRound, this.playerPoints);
            }
        }
        */

        private string[] GetWinningPlayerIds()
        {
            Dictionary<string, int> playerPointsSum = new Dictionary<string, int>
            {
                [Constants.PlayerIds.Player1] = this.playerPoints[Constants.PlayerIds.Player1].Sum(),
                [Constants.PlayerIds.Player2] = this.playerPoints[Constants.PlayerIds.Player2].Sum(),
                [Constants.PlayerIds.Player3] = this.playerPoints[Constants.PlayerIds.Player3].Sum(),
                [Constants.PlayerIds.Player4] = this.playerPoints[Constants.PlayerIds.Player4].Sum(),
            };

            var minPoints = playerPointsSum.Min(selector => selector.Value);

            return playerPointsSum
                .Where(playerPoints => playerPoints.Value == minPoints)
                .Select(points => points.Key)
                .ToArray();
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
}