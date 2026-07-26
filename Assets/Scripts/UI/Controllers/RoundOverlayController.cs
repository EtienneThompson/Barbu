namespace Barbu.UI.Controllers
{
    using System;
    using System.Collections;
    using System.Linq;
    using Barbu.Core.Events;
    using Barbu.Core.Telemetry;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class RoundOverlayController : MonoBehaviour, IRoundOverlayController
    {
        private IEventsController eventsController;
        private ITelemetryService telemetryService;

        private TextMeshProUGUI roundText;
        private TextMeshProUGUI subtitleText;

        [Inject]
        public void Init(IEventsController eventsController, ITelemetryService telemetryService)
        {
            this.eventsController = eventsController;
            this.telemetryService = telemetryService;
        }

        public void OnEnable()
        {
            this.telemetryService.LogInfo("Round Overlay Controller OnEnable");
            var roundObject = GameObjectExtensions.FindGameObjectByName("RoundOverlayTitle");
            this.telemetryService.LogInfo(roundObject?.ToString());
            this.roundText = roundObject?.GetComponent<TextMeshProUGUI>();
            var subtitleObject = GameObjectExtensions.FindGameObjectByName("RoundOverlaySubtitle");
            this.subtitleText = subtitleObject?.GetComponent<TextMeshProUGUI>();
        }

        public void DisplayRound(string roundName, GameTypes gameType)
        {
            this.roundText.fontSize = 36;
            this.subtitleText.fontSize = 24;
            StartCoroutine(DisplayRoutine(
                roundName,
                subText: gameType.ToString(),
                delay: 1.5f,
                callback: () => this.eventsController.Fire(EventNames.RoundAnimationFinished)));
        }

        public void DisplayWinner(string[] winningPlayers)
        {
            this.telemetryService.LogInfo("Displaying winner");
            var playerLabels = winningPlayers
                .Select(playerId => playerId.Equals(Constants.PlayerIds.Player1) ? "You" : $"Player {playerId}")
                .ToArray();

            // "You", "Player 2", "You and Player 2", "You, Player 2 and Player 3"
            string winningLabel = playerLabels.Length > 1
                ? string.Join(", ", playerLabels.Take(playerLabels.Length - 1)) + " and " + playerLabels.Last()
                : playerLabels.FirstOrDefault();

            // A single computer player "wins"; "you win" and multiple players "win".
            bool isPlural = playerLabels.Length > 1 ||
                winningPlayers.FirstOrDefault()?.Equals(Constants.PlayerIds.Player1) == true;
            winningLabel += isPlural ? " win!" : " wins!";
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
            this.roundText.fontSize = 32;
            this.subtitleText.fontSize = 18;
            this.roundText.text = string.Empty;
            this.subtitleText.text = string.Empty;
        }

        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
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
            this.roundText.text = mainText;
            this.subtitleText.text = subText;
        }
    }
}