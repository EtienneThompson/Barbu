namespace Barbu.UI.Controllers
{
    using System;
    using System.Collections;
    using System.Linq;
    using Barbu.Core;
    using Barbu.Models;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Zenject;

    public class RoundOverlayController : MonoBehaviour
    {
        private IEventsController eventsController;
        private ITelemetryService telemetryService;
        private Label roundLabel;
        private Label subtitleLabel;

        [Inject]
        public void Init(IEventsController eventsController, ITelemetryService telemetryService)
        {
            this.eventsController = eventsController;
            this.telemetryService = telemetryService;
        }

        public void OnEnable()
        {
            var roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
            var document = roundOverlay.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            this.roundLabel = root.Q<Label>("round");
            this.subtitleLabel = root.Q<Label>("subtitle");
        }

        public void DisplayRound(string roundName, GameTypes gameType)
        {
            StartCoroutine(DisplayRoutine(
                roundName,
                subText: gameType.ToString(),
                delay: 1.5f,
                callback: () => this.eventsController.Fire(EventNames.RoundAnimationFinished)));
        }

        public void DisplayWinner(string[] winningPlayers)
        {
            this.telemetryService.LogInfo("Displaying winner");
            var playerLabels = winningPlayers.Select(playerId => playerId.Equals(Constants.PlayerIds.Player1) ? "You" : $"Player {playerId}");
            string winningLabel = string.Empty;
            foreach (var playerLabel in playerLabels)
            {
                if (playerLabels.Count() > 1 &&
                    playerLabel.Equals(playerLabels.ElementAt(playerLabels.Count() - 1)))
                {
                    winningLabel = winningLabel.Substring(0, winningLabel.Length - 2) + " and ";
                }
                winningLabel += playerLabel + ", ";
            }
            winningLabel = winningLabel.Substring(0, winningLabel.Length - 2) + " win";
            winningLabel += (
                playerLabels.Count() > 1 || (
                    playerLabels.Count() == 1 && 
                    playerLabels.ElementAt(0).Equals(Constants.PlayerIds.Player1)))
                ? "!"
                : "s!";
            this.telemetryService.LogInfo(winningLabel);
            StartCoroutine(DisplayRoutine(
                winningLabel,
                delay: 2.5f,
                callback: () => this.eventsController.Fire(EventNames.WinnerAnimationFinished)));
        }

        public void ShowRoundOverMessage()
        {
            this.ShowText("Round Over! All points earned.", subText: "Automatically playing remaining cards.");
        }

        public void HideText()
        {
            this.roundLabel.text = string.Empty;
            this.subtitleLabel.text = string.Empty;
        }

        private IEnumerator DisplayRoutine(string mainText, string subText = null, float delay = 1.5f, Action callback = null)
        {
            this.telemetryService.LogInfo(mainText);
            this.ShowText(mainText, subText);
            yield return new WaitForSeconds(delay);
            this.HideText();
            if (callback != null)
            {
                callback();
            }
        }

        private void ShowText(string mainText, string subText = null)
        {
            this.roundLabel.text = mainText;
            this.subtitleLabel.text = subText;
        }
    }
}