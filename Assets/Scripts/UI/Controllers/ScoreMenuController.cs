namespace Barbu.UI.Controllers
{
    using Barbu.Core;
    using Barbu.Core.Events;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class ScoreMenuController : MonoBehaviour
    {
        private const float BaseAnimationTime = 1.0f;

        private Transform ScoreContainer;
        private Transform ScoreRowTemplate;
        private Transform TotalRow;
        private List<Transform> ScoreRows;

        private IStateMachine stateMachine;
        private IEventsController eventsController;
        private GameObject inGamePoints;
        private GameObject roundOverlay;

        [Inject]
        public void Init(IStateMachine stateMachine, IEventsController eventsController)
        {
            this.stateMachine = stateMachine;
            this.eventsController = eventsController;
        }

        public void OnEnable()
        {
            this.inGamePoints = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.InGamePoints, findInactive: true);
            this.roundOverlay = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.RoundOverlay, findInactive: true);

            this.ScoreRows = new List<Transform>();
            this.ScoreContainer = transform.Find("ScoreContainer");
            this.ScoreRowTemplate = this.ScoreContainer.Find("ScoreRowTemplate");
            this.TotalRow = this.ScoreContainer.Find("TotalSection");

            // Hide the template object by default.
            this.ScoreRowTemplate.gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            foreach (Transform scoreRow in this.ScoreRows)
            {
                Destroy(scoreRow.gameObject);
            }
        }

        void Update()
        {
            // User acknowledges the score and moves on to the next round.
            if (gameObject.activeSelf && Input.GetMouseButtonDown(0))
            {
                gameObject.SetActive(false);
                if (this.stateMachine.IsMenuOpen)
                {
                    this.stateMachine.SetMenuOpen(false);
                    this.inGamePoints.SetActive(true);
                    this.roundOverlay.SetActive(true);
                    this.eventsController.Fire(EventNames.ResumeGame);
                    this.gameObject.SetActive(false);
                }
                else
                {
                    this.eventsController.Fire(EventNames.ScoreMenuDismissed);
                }
            }
        }

        public void DisplayScores(
            Dictionary<string, int[]> scores,
            int currentRoundIndex,
            bool isPositiveRound = false,
            bool skipAnimations = false)
        {
            this.gameObject.SetActive(true);

            var player1Total = 0;
            var player2Total = 0;
            var player3Total = 0;
            var player4Total = 0;
            for (int i = 0; i < scores[Constants.PlayerIds.Player1].Length; i++)
            {
                Transform scoreRow = Instantiate(this.ScoreRowTemplate, this.ScoreContainer);
                RectTransform rectTransform = scoreRow.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, 58.5f - (58.5f * i));
                scoreRow.gameObject.SetActive(true);
                this.ScoreRows.Add(scoreRow);

                scoreRow.Find("RoundLabel").GetComponent<TMP_Text>().text = (i + 1).ToString();
                var player1Score = scores[Constants.PlayerIds.Player1][i];
                var player2Score = scores[Constants.PlayerIds.Player2][i];
                var player3Score = scores[Constants.PlayerIds.Player3][i];
                var player4Score = scores[Constants.PlayerIds.Player4][i];
                player1Total += player1Score;
                player2Total += player2Score;
                player3Total += player3Score;
                player4Total += player4Score;
                this.SetScore(scoreRow, "Player1Label", player1Score, playAnimation: !skipAnimations && i == currentRoundIndex, isPositiveRound: isPositiveRound);
                this.SetScore(scoreRow, "Player2Label", player2Score, playAnimation: !skipAnimations && i == currentRoundIndex, isPositiveRound: isPositiveRound);
                this.SetScore(scoreRow, "Player3Label", player3Score, playAnimation: !skipAnimations && i == currentRoundIndex, isPositiveRound: isPositiveRound);
                this.SetScore(scoreRow, "Player4Label", player4Score, playAnimation: !skipAnimations && i == currentRoundIndex, isPositiveRound: isPositiveRound);
            }

            var player1Previous = currentRoundIndex == 0 ? 0 : player1Total - scores[Constants.PlayerIds.Player1][currentRoundIndex];
            var player2Previous = currentRoundIndex == 0 ? 0 : player2Total - scores[Constants.PlayerIds.Player2][currentRoundIndex];
            var player3Previous = currentRoundIndex == 0 ? 0 : player3Total - scores[Constants.PlayerIds.Player3][currentRoundIndex];
            var player4Previous = currentRoundIndex == 0 ? 0 : player4Total - scores[Constants.PlayerIds.Player4][currentRoundIndex];

            this.SetScore(this.TotalRow, "Player1Label", player1Total, baseScore: player1Previous, playAnimation: !skipAnimations && true, isPositiveRound: isPositiveRound);
            this.SetScore(this.TotalRow, "Player2Label", player2Total, baseScore: player2Previous, playAnimation: !skipAnimations && true, isPositiveRound: isPositiveRound);
            this.SetScore(this.TotalRow, "Player3Label", player3Total, baseScore: player3Previous, playAnimation: !skipAnimations && true, isPositiveRound: isPositiveRound);
            this.SetScore(this.TotalRow, "Player4Label", player4Total, baseScore: player4Previous, playAnimation: !skipAnimations && true, isPositiveRound: isPositiveRound);
        }

        private void SetScore(
            Transform parent,
            string componentLabel,
            int score,
            int baseScore = 0,
            bool playAnimation = false,
            bool isPositiveRound = false)
        {
            var textComponent = parent.Find(componentLabel).GetComponent<TMP_Text>();
            if (playAnimation)
            {
                StartCoroutine(IncreaseScore(
                    textComponent, score, baseScore: baseScore, isPositiveRound: isPositiveRound));
            }
            else
            {
                textComponent.text = score.ToString();
            }
        }

        private System.Collections.IEnumerator IncreaseScore(
            TMP_Text component,
            int score,
            int baseScore = 0,
            bool isPositiveRound = false)
        {
            component.text = baseScore.ToString();

            if (!isPositiveRound)
            {
                var remainingScore = score - baseScore;
                var waitPerIncrement = BaseAnimationTime / remainingScore;
                int i = baseScore;
                while (i <= score)
                {
                    component.text = i.ToString();
                    i++;
                    yield return new WaitForSeconds(waitPerIncrement);
                }
            }
            else
            {
                var waitPerIncrement = BaseAnimationTime / Math.Abs(score);
                int i = baseScore;
                while (i >= score)
                {
                    component.text = i.ToString();
                    i--;
                    yield return new WaitForSeconds(waitPerIncrement);
                }
            }
        }
    }
}
