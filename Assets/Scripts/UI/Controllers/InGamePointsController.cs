namespace Barbu.UI.Controllers
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class InGamePointsController : MonoBehaviour
    {
        private TextMeshProUGUI player1points;
        private TextMeshProUGUI player2points;
        private TextMeshProUGUI player3points;
        private TextMeshProUGUI player4points;
        private Dictionary<string, TextMeshProUGUI> playerPoints;
        //private Label roundName;

        private void OnEnable()
        {
            this.player1points = this.GetTextComponent("Player1Points");
            this.player2points = this.GetTextComponent("Player2Points");
            this.player3points = this.GetTextComponent("Player3Points");
            this.player4points = this.GetTextComponent("Player4Points");

            this.ResetPoints();

            this.playerPoints = new Dictionary<string, TextMeshProUGUI>
            {
                [Constants.PlayerIds.Player1] = this.player1points,
                [Constants.PlayerIds.Player2] = this.player2points,
                [Constants.PlayerIds.Player3] = this.player3points,
                [Constants.PlayerIds.Player4] = this.player4points,
            };
        }

        public void SetRoundName(string roundName)
        {
            //this.roundName.text = roundName;
            //this.roundName.style.display = DisplayStyle.Flex;
        }

        public void ResetRoundName()
        {
            //this.roundName.text = string.Empty;
            //this.roundName.style.display = DisplayStyle.None;
        }

        public void UpdatePlayerPoints(string player, int points)
        {
            var currentPoints = int.Parse(this.playerPoints[player].text);
            this.playerPoints[player].text = $"{(currentPoints + points).ToString()} Pts";
        }

        public void ResetPoints()
        {
            this.player1points.text = "0 Pts";
            this.player2points.text = "0 Pts";
            this.player3points.text = "0 Pts";
            this.player4points.text = "0 Pts";
        }

        private TextMeshProUGUI GetTextComponent(string gameObjectName)
        {
            var gameObject = GameObjectExtensions.FindGameObjectByName(gameObjectName);
            return gameObject.GetComponent<TextMeshProUGUI>();
        }
    }
}
