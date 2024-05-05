namespace Barbu.UI.Controllers
{
    using Barbu.Core;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class SingleRoundMenuController : MonoBehaviour
    {
        private StateMachine stateMachine;
        private GameBoard gameBoard;
        private GameObject singleRoundMenu;
        private GameObject gamesMenu;
        private Button heartsBtn;
        private Button queensBtn;
        private Button kingOfHeartsBtn;
        private Button pilesBtn;
        private Button nothingBtn;
        private Button everythingBtn;
        private Button closeBtn;

        public void OnEnable()
        {
            this.stateMachine = new StateMachine();
            this.stateMachine.SetMenuOpen(true);
            this.gameBoard = GameObject.Find(Constants.GameObjects.GameBoard).GetComponent<GameBoard>();
            this.singleRoundMenu = GameObject.Find(Constants.GameObjects.SingleRoundMenu);
            this.gamesMenu = GameObject.Find(Constants.GameObjects.GamesMenu);
            var document = this.singleRoundMenu.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            this.heartsBtn = root.Q<Button>("hearts");
            this.queensBtn = root.Q<Button>("queens");
            this.kingOfHeartsBtn = root.Q<Button>("kingofhearts");
            this.pilesBtn = root.Q<Button>("piles");
            this.nothingBtn = root.Q<Button>("nothing");
            this.everythingBtn = root.Q<Button>("everything");
            this.closeBtn = root.Q<Button>("close");

            this.heartsBtn.RegisterCallback<ClickEvent>(HandleHeartsButtonClick);
            this.queensBtn.RegisterCallback<ClickEvent>(HandleQueensButtonClick);
            this.kingOfHeartsBtn.RegisterCallback<ClickEvent>(HandleKingOfHeartsButtonClick);
            this.pilesBtn.RegisterCallback<ClickEvent>(HandlePilesButtonClick);
            this.nothingBtn.RegisterCallback<ClickEvent>(HandleNothingButtonClick);
            this.everythingBtn.RegisterCallback<ClickEvent>(HandleEverythingButtonClick);
            this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        public void OnDisable()
        {
            this.stateMachine.SetMenuOpen(false);
            this.heartsBtn.UnregisterCallback<ClickEvent>(HandleHeartsButtonClick);
            this.queensBtn.UnregisterCallback<ClickEvent>(HandleQueensButtonClick);
            this.kingOfHeartsBtn.UnregisterCallback<ClickEvent>(HandleKingOfHeartsButtonClick);
            this.pilesBtn.UnregisterCallback<ClickEvent>(HandlePilesButtonClick);
            this.nothingBtn.UnregisterCallback<ClickEvent>(HandleNothingButtonClick);
            this.everythingBtn.UnregisterCallback<ClickEvent>(HandleEverythingButtonClick);
            this.closeBtn.UnregisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        private void HandleHeartsButtonClick(ClickEvent evt)
        {
            this.singleRoundMenu.SetActive(false);
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Hearts);
        }

        private void HandleQueensButtonClick(ClickEvent evt)
        {
            this.singleRoundMenu.SetActive(false);
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Queens);
        }

        private void HandleKingOfHeartsButtonClick(ClickEvent evt)
        {
            this.singleRoundMenu.SetActive(false);
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.KingOfHearts);
        }

        private void HandlePilesButtonClick(ClickEvent evt)
        {
            this.singleRoundMenu.SetActive(false);
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Piles);
        }

        private void HandleNothingButtonClick(ClickEvent evt)
        {
            this.singleRoundMenu.SetActive(false);
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Nothing);
        }

        private void HandleEverythingButtonClick(ClickEvent evt)
        {
            this.singleRoundMenu.SetActive(false);
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Everything);
        }

        private void HandleCloseButtonClick(ClickEvent evt)
        {
            this.singleRoundMenu.SetActive(false);
        }
    }
}
