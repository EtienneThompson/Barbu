using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundOverlayController : MonoBehaviour
{
    private EventsController eventsController;
    private Label roundLabel;

    public void OnEnable()
    {
        this.eventsController = EventsController.GetInstance();
        var roundOverlay = GameObject.Find(Constants.GameObjects.RoundOverlay);
        var document = roundOverlay.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.roundLabel = root.Q<Label>("round");
    }

    public void DisplayRound(string roundName)
    {
        StartCoroutine(DisplayRoutine(roundName, 1.5f, this.eventsController.FinishRoundAnimation));
    }

    public void DisplayWinner(string[] winningPlayers)
    {
        Debug.Log("Displaying winner");
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
        winningLabel = winningLabel.Substring(0, winningLabel.Length - 2) + " win" + (playerLabels.Count() > 1 ? "!" : "s!");
        Debug.Log(winningLabel);
        StartCoroutine(DisplayRoutine(winningLabel, 2.5f, this.eventsController.FinishWinnerAnimation));
    }

    IEnumerator DisplayRoutine(string roundName, float delay, Action action)
    {
        Debug.Log(roundName);
        this.roundLabel.text = roundName;
        yield return new WaitForSeconds(delay);
        this.roundLabel.text = string.Empty;
        action();
    }
}
