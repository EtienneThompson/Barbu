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

        private EventsController eventsController;
        private List<Transform> ScoreRows;

        public void OnEnable()
        {
            this.eventsController = EventsController.GetInstance();
            this.ScoreRows = new List<Transform>();

            this.ScoreContainer = transform.Find("ScoreContainer");
            this.ScoreRowTemplate = this.ScoreContainer.Find("ScoreRowTemplate");

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

            for (int i = 0; i < scores[Constants.PlayerIds.Player1].Length; i++)
            {
                Transform scoreRow = Instantiate(this.ScoreRowTemplate, this.ScoreContainer);
                RectTransform rectTransform = scoreRow.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, 58.5f - (58.5f * i));
                scoreRow.gameObject.SetActive(true);
                this.ScoreRows.Add(scoreRow);

                Debug.Log(scoreRow.Find("RoundLabel").GetComponent<TMP_Text>());
                scoreRow.Find("RoundLabel").GetComponent<TMP_Text>().text = (i + 1).ToString();
                scoreRow.Find("Player1Label").GetComponent<TMP_Text>().text = scores[Constants.PlayerIds.Player1][i].ToString();
                scoreRow.Find("Player2Label").GetComponent<TMP_Text>().text = scores[Constants.PlayerIds.Player2][i].ToString();
                scoreRow.Find("Player3Label").GetComponent<TMP_Text>().text = scores[Constants.PlayerIds.Player3][i].ToString();
                scoreRow.Find("Player4Label").GetComponent<TMP_Text>().text = scores[Constants.PlayerIds.Player4][i].ToString();
            }
        }
    }
}
