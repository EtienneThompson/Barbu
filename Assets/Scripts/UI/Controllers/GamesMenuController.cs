namespace Barbu.UI.Controllers
{

    using Barbu;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class GamesMenuController : MonoBehaviour
    {
        private StateMachine stateMachine;
        private GameBoard gameBoard;
        private GameObject gamesMenu;
        private GameObject singleRoundMenu;
        private Button traditionalBtn;
        private Button singleBtn;
        private Button chaosBtn;
        private Button closeBtn;

        public void OnEnable()
        {
            this.stateMachine = new StateMachine();
            this.stateMachine.SetMenuOpen(true);
            this.gameBoard = GameObject.Find(Constants.GameObjects.GameBoard).GetComponent<GameBoard>();
            this.gamesMenu = GameObject.Find(Constants.GameObjects.GamesMenu);
            this.singleRoundMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SingleRoundMenu, true);

            var document = this.gamesMenu.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            this.traditionalBtn = root.Q<Button>("traditional");
            this.singleBtn = root.Q<Button>("single");
            this.chaosBtn = root.Q<Button>("chaos");
            this.closeBtn = root.Q<Button>("close");

            this.traditionalBtn.RegisterCallback<ClickEvent>(HandleTraditionalButtonClick);
            this.singleBtn.RegisterCallback<ClickEvent>(HandleSingleButtonClick);
            this.chaosBtn.RegisterCallback<ClickEvent>(HandleChaosButtonClick);
            this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        public void OnDisable()
        {
            this.traditionalBtn.UnregisterCallback<ClickEvent>(HandleTraditionalButtonClick);
            this.singleBtn.UnregisterCallback<ClickEvent>(HandleSingleButtonClick);
            this.chaosBtn.UnregisterCallback<ClickEvent>(HandleChaosButtonClick);
            this.closeBtn.UnregisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        private void HandleTraditionalButtonClick(ClickEvent evt)
        {
            this.gameBoard.CreateNewGame(Constants.TraditionalRoundManager.GameName, null);
            this.gamesMenu.SetActive(false);
            this.stateMachine.SetMenuOpen(false);
        }

        private void HandleSingleButtonClick(ClickEvent evt)
        {
            this.gamesMenu.SetActive(false);
            this.singleRoundMenu.SetActive(true);
        }

        private void HandleChaosButtonClick(ClickEvent evt)
        {
            this.gameBoard.CreateNewGame(Constants.ChaosRoundManager.GameName, null);
            this.gamesMenu.SetActive(false);
            this.stateMachine.SetMenuOpen(false);
        }

        private void HandleCloseButtonClick(ClickEvent evt)
        {
            this.gamesMenu.SetActive(false);
            this.stateMachine.SetMenuOpen(false);
        }
    }
}