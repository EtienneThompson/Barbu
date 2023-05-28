using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGamePointsController : MonoBehaviour
{
    private Label player1points;
    private Label player2points;
    private Label player3points;
    private Label player4points;
    private Dictionary<string, Label> playerPoints;

    private void OnEnable()
    {
        var inGamePoints = GameObject.Find(Constants.GameObjects.InGamePoints);
        var document = inGamePoints.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.player1points = root.Q<Label>("player1points");
        this.player2points = root.Q<Label>("player2points");
        this.player3points = root.Q<Label>("player3points");
        this.player4points = root.Q<Label>("player4points");

        this.ResetPoints();

        this.playerPoints = new Dictionary<string, Label>
        {
            [Constants.PlayerIds.Player1] = this.player1points,
            [Constants.PlayerIds.Player2] = this.player2points,
            [Constants.PlayerIds.Player3] = this.player3points,
            [Constants.PlayerIds.Player4] = this.player4points,
        };
    }

    public void UpdatePlayerPoints(string player, int points)
    {
        var currentPoints = int.Parse(this.playerPoints[player].text);
        this.playerPoints[player].text = (currentPoints + points).ToString();
    }

    public void ResetPoints()
    {
        this.player1points.text = "0";
        this.player2points.text = "0";
        this.player3points.text = "0";
        this.player4points.text = "0";
    }
}
