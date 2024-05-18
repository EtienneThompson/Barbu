namespace Barbu.UI.Controllers
{
    using Barbu.Core;
    using Barbu.Models;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class ScoreMenuController : MonoBehaviour
    {
        private Transform ScoreContainer;
        private Transform ScoreRowTemplate;
        private Transform TotalRow;

        private EventsController eventsController;
        private List<Transform> ScoreRows;

        public void OnEnable()
        {
            this.eventsController = EventsController.GetInstance();
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
                this.eventsController.Fire(EventNames.RoundOver);
            }
        }

        public void DisplayScores(Dictionary<string, int[]> scores)
        {
            gameObject.SetActive(true);

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
                scoreRow.Find("Player1Label").GetComponent<TMP_Text>().text = player1Score.ToString();
                scoreRow.Find("Player2Label").GetComponent<TMP_Text>().text = player2Score.ToString();
                scoreRow.Find("Player3Label").GetComponent<TMP_Text>().text = player3Score.ToString();
                scoreRow.Find("Player4Label").GetComponent<TMP_Text>().text = player4Score.ToString();
            }

            this.TotalRow.Find("Player1Label").GetComponent<TMP_Text>().text = player1Total.ToString();
            this.TotalRow.Find("Player2Label").GetComponent<TMP_Text>().text = player2Total.ToString();
            this.TotalRow.Find("Player3Label").GetComponent<TMP_Text>().text = player3Total.ToString();
            this.TotalRow.Find("Player4Label").GetComponent<TMP_Text>().text = player4Total.ToString();
        }
    }
}
