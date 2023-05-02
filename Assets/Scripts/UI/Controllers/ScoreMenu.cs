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

        var playerRound5 = root.Q<Label>("playerRound5");
        var computer1Round5 = root.Q<Label>("computer1Round5");
        var computer2Round5 = root.Q<Label>("computer2Round5");
        var computer3Round5 = root.Q<Label>("computer3Round5");
        var round5Labels = new Label[] { playerRound5, computer1Round5, computer2Round5, computer3Round5 };
        this.roundLabelMap.Add(4, round5Labels);

        var playerRound6 = root.Q<Label>("playerRound6");
        var computer1Round6 = root.Q<Label>("computer1Round6");
        var computer2Round6 = root.Q<Label>("computer2Round6");
        var computer3Round6 = root.Q<Label>("computer3Round6");
        var round6Labels = new Label[] { playerRound6, computer1Round6, computer2Round6, computer3Round6 };
        this.roundLabelMap.Add(5, round6Labels);

        var playerTotal = root.Q<Label>("playerTotal");
        var computer1Total = root.Q<Label>("computer1Total");
        var computer2Total = root.Q<Label>("computer2Total");
        var computer3Total = root.Q<Label>("computer3Total");
        var totalLabels = new Label[] { playerTotal, computer1Total, computer2Total, computer3Total };
        this.roundLabelMap.Add(6, totalLabels);
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
        var total0Text = 0;
        var total1Text = 0;
        var total2Text = 0;
        var total3Text = 0;
        for (int i = 0; i < round + 1; i++)
        {
            var labels = this.roundLabelMap[i];
            labels[0].text = playerPoints["1"][i].ToString();
            labels[1].text = playerPoints["2"][i].ToString();
            labels[2].text = playerPoints["3"][i].ToString();
            labels[3].text = playerPoints["4"][i].ToString();

            total0Text += playerPoints["1"][i];
            total1Text += playerPoints["2"][i];
            total2Text += playerPoints["3"][i];
            total3Text += playerPoints["4"][i];
        }

        Debug.Log("Updating total points");
        var totals = this.roundLabelMap[6];
        totals[0].text = total0Text.ToString();
        totals[1].text = total1Text.ToString();
        totals[2].text = total2Text.ToString();
        totals[3].text = total3Text.ToString();
    }
}
