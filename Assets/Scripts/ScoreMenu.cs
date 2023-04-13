using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreMenu : MonoBehaviour
{
    private int currentRound;
    public delegate void OnRoundOver();
    public static OnRoundOver onRoundOver;

    private Dictionary<int, Label[]> roundLabelMap;

    public void OnEnable()
    {
        Debug.Log("onEnable");
        GameObject scoreBoard = GameObject.Find("ScoreMenuCanvas");
        var document = scoreBoard.GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        
        this.roundLabelMap = new Dictionary<int, Label[]>();

        var playerRound1 = root.Q<Label>("playerRound1");
        Debug.Log(playerRound1);
        var computer1Round1 = root.Q<Label>("computer1Round1");
        var computer2Round1 = root.Q<Label>("computer2Round1");
        var computer3Round1 = root.Q<Label>("computer3Round1");
        var round1Labels = new Label[] { playerRound1, computer1Round1, computer2Round1, computer3Round1 };
        this.roundLabelMap.Add(0, round1Labels);

        var playerRound2 = root.Q<Label>("playerRound2");
        var computer1Round2 = root.Q<Label>("computer1Round2");
        var computer2Round2 = root.Q<Label>("computer2Round2");
        var computer3Round2 = root.Q<Label>("computer3Round2");
        var round2Labels = new Label[] { playerRound2, computer1Round2, computer2Round2, computer3Round2 };
        this.roundLabelMap.Add(1, round2Labels);

        var playerRound3 = root.Q<Label>("playerRound3");
        var computer1Round3 = root.Q<Label>("computer1Round3");
        var computer2Round3 = root.Q<Label>("computer2Round3");
        var computer3Round3 = root.Q<Label>("computer3Round3");
        var round3Labels = new Label[] { playerRound3, computer1Round3, computer2Round3, computer3Round3 };
        this.roundLabelMap.Add(2, round3Labels);

        var playerRound4 = root.Q<Label>("playerRound4");
        var computer1Round4 = root.Q<Label>("computer1Round4");
        var computer2Round4 = root.Q<Label>("computer2Round4");
        var computer3Round4 = root.Q<Label>("computer3Round4");
        var round4Labels = new Label[] { playerRound4, computer1Round4, computer2Round4, computer3Round4 };
        this.roundLabelMap.Add(3, round4Labels);

        var playerTotal = root.Q<Label>("playerTotal");
        var computer1Total = root.Q<Label>("computer1Total");
        var computer2Total = root.Q<Label>("computer2Total");
        var computer3Total = root.Q<Label>("computer3Total");
        var totalLabels = new Label[] { playerTotal, computer1Total, computer2Total, computer3Total };
        this.roundLabelMap.Add(4, totalLabels);
    }

    void Update()
    {
        // User acknowledges the score and moves on to the next round.
        if (gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);

            if (this.currentRound + 1 != Constants.MaxRounds)
            {
                Debug.Log("Going to next round!");
                onRoundOver();
            }
            else
            {
                Debug.Log("GAME OVER!!!");
            }
        }
    }

    public void UpdateScores(int round, Dictionary<string, int[]> playerPoints)
    {
        gameObject.SetActive(true);
        this.currentRound = round;
        Debug.Log("Updating scores for round " + round);
        Debug.Log("Player points: " + playerPoints["1"][round]);
        Debug.Log("Computer 1 points: " + playerPoints["2"][round]);
        Debug.Log("Computer 2 points: " + playerPoints["3"][round]);
        Debug.Log("Computer 3 points: " + playerPoints["4"][round]);
        var labels = this.roundLabelMap[round];
        Debug.Log("Updating round points");
        Debug.Log(labels[0]);
        labels[0].text = playerPoints["1"][round].ToString();
        labels[1].text = playerPoints["2"][round].ToString();
        labels[2].text = playerPoints["3"][round].ToString();
        labels[3].text = playerPoints["4"][round].ToString();

        Debug.Log("Updating total points");
        var totals = this.roundLabelMap[4];
        var total0Text = string.IsNullOrEmpty(totals[0].text) ? "0" : totals[0].text;
        var total1Text = string.IsNullOrEmpty(totals[1].text) ? "0" : totals[1].text;
        var total2Text = string.IsNullOrEmpty(totals[2].text) ? "0" : totals[2].text;
        var total3Text = string.IsNullOrEmpty(totals[3].text) ? "0" : totals[3].text;
        totals[0].text = (Int32.Parse(total0Text) + playerPoints["1"][round]).ToString();
        totals[1].text = (Int32.Parse(total1Text) + playerPoints["2"][round]).ToString();
        totals[2].text = (Int32.Parse(total2Text) + playerPoints["3"][round]).ToString();
        totals[3].text = (Int32.Parse(total3Text) + playerPoints["4"][round]).ToString();
    }
}
