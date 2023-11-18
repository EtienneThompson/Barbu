using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGamePointsController : MonoBehaviour
{
    private Image settingsButtonImage;
    private Image newGameButtonImage;
    private Image rulesButtonImage;
    private Label player1points;
    private Label player2points;
    private Label player3points;
    private Label player4points;
    private Dictionary<string, Label> playerPoints;
    private Label roundName;

    private void OnEnable()
    {
        var inGamePoints = GameObject.Find(Constants.GameObjects.InGamePoints);
        var document = inGamePoints.GetComponent<UIDocument>();
        var root = document.rootVisualElement;

        this.settingsButtonImage = root.Q<Image>("settingsButtonImage");
        var settingsIcon = Resources.Load<Texture2D>("Icons/Buttons/settings_button");
        this.settingsButtonImage.image = settingsIcon;

        this.newGameButtonImage = root.Q<Image>("newGameButtonImage");
        var newGameIcon = Resources.Load<Texture2D>("Icons/Buttons/new_game_button");
        this.newGameButtonImage.image = newGameIcon;

        this.rulesButtonImage = root.Q<Image>("rulesButtonImage");
        var rulesIcon = Resources.Load<Texture2D>("Icons/Buttons/rules_button");
        this.rulesButtonImage.image = rulesIcon;

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

        this.roundName = root.Q<Label>("round-name");
    }

    public void SetRoundName(string roundName)
    {
        this.roundName.text = roundName;
        this.roundName.style.display = DisplayStyle.Flex;
    }

    public void ResetRoundName()
    {
        this.roundName.text = string.Empty;
        this.roundName.style.display = DisplayStyle.None;
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
