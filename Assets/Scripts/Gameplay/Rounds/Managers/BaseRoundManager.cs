namespace Barbu.Gameplay.Rounds.Managers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Barbu.Core;
    using Barbu.Interfaces;
    using Barbu.Interfaces.Rounds;
    using Barbu.Models;
    using Barbu.UI.Controllers;
    using UnityEngine;
    using Barbu.Interfaces.Core;
    using Barbu.Core.Workflows.PlayTrickWorkflow;

    public class BaseRoundManager : IRoundManager, IEventListener
    {
        protected int pilesPlayed = 0;
        protected string roundStartingPlayerId = Constants.PlayerIds.Player1;
        protected int currentRound = 0;
        protected int totalRounds;

        protected RoundContext roundContext;
        protected PlayTrickWorkflow playTrickWorkflow;
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
            this.hands = hands;
            this.stateMachine = new StateMachine();
            this.eventsController = EventsController.GetInstance();
            this.stateMachine.SetStartingSuit(string.Empty);
            this.telemetryService = TelemetryService.GetInstance();
            this.scoreMenu = scoreMenu;
            this.gameBoard = gameBoard;
            this.inGamePointsController = inGamePointsController;
            this.totalRounds = totalRounds;
            this.advertisementController = this.gameBoard.GetComponent<AdvertisementController>();
            this.advertisementController.RequestToShowInterstitial();

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

            var startingPlayerId = this.roundStartingPlayerId;
            if (this.playTrickWorkflow != null)
            {
                this.telemetryService.LogInfo($"[BaseRoundManager] Setting winning player: {this.playTrickWorkflow.GetWinningPlayerId()}");
                startingPlayerId = this.playTrickWorkflow.GetWinningPlayerId();
            }

            this.playTrickWorkflow = new PlayTrickWorkflow(
                this.roundContext,
                this.inGamePointsController,
                this.playerPoints,
                this.hands,
                Int32.Parse(startingPlayerId) - 1,
                this.currentRound);
            this.playTrickWorkflow.StartAsync();

            this.pilesPlayed++;
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
            this.playTrickWorkflow = null;
            this.hands = hands;
            this.currentRound++;
            this.roundContext.Next();
            var currentStartingPlayer = Int32.Parse(this.roundStartingPlayerId);
            this.telemetryService.LogInfo("Last round starting player: " + this.roundStartingPlayerId);
            var newStartingPlayer = (currentStartingPlayer % 4) + 1;
            this.telemetryService.LogInfo("New starting player id: " + newStartingPlayer);
            this.roundStartingPlayerId = newStartingPlayer.ToString();
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
            this.eventsController.Subscribe(EventNames.RoundOver, this.CleanupRound);
            this.eventsController.Subscribe(EventNames.RoundAnimationFinished, this.StartRound);
            this.eventsController.Subscribe(EventNames.PileResolved, this.StartRound);
            this.eventsController.Subscribe(EventNames.WinnerAnimationFinished, this.OnWinnerDisplayed);
        }

        public void Destroy()
        {
            // Deregister event listeners when this round is no longer applicable.
            this.eventsController.Unsubscribe(EventNames.RoundOver, this.CleanupRound);
            this.eventsController.Unsubscribe(EventNames.RoundAnimationFinished, this.StartRound);
            this.eventsController.Unsubscribe(EventNames.PileResolved, this.StartRound);
            this.eventsController.Unsubscribe(EventNames.WinnerAnimationFinished, this.OnWinnerDisplayed);
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
    }
}