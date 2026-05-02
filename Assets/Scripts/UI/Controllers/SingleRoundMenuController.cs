namespace Barbu.UI.Controllers
{
    using Barbu;
    using Barbu.Core;
    using Barbu.Core.Telemetry;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class SingleRoundMenuController : MonoBehaviour
    {
        private GameObject singleRoundMenuContainer;
        private Button heartsButton;
        private Button queensButton;
        private Button kingOfHeartsButton;
        private Button pilesButton;
        private Button nothingButton;
        private Button everythingButton;

        private GameBoard gameBoard;
        private MainMenuController mainMenuController;
        private IStateMachine stateMachine;
        private ITelemetryService telemetryService;

        private const float menuSlideSpeed = 300f;
        private const float menuSlideOutSpeedWhenExtended = 600f;

        private Vector2 GetInViewPosition() =>
            Settings.MenuSidePreference == Settings.MenuSide.Right
                ? new Vector2(-110, 0)
                : new Vector2(110, 0);

        private Vector2 GetBehindMenuPosition() =>
            Settings.MenuSidePreference == Settings.MenuSide.Right
                ? new Vector2(-45, 0)
                : new Vector2(45, 0);

        private Vector2 GetOffscreenPosition() =>
            Settings.MenuSidePreference == Settings.MenuSide.Right
                ? new Vector2(45, 0)
                : new Vector2(-45, 0);

        public void ApplyMenuSide()
        {
            var rt = this.singleRoundMenuContainer.GetComponent<RectTransform>();
            bool isRight = Settings.MenuSidePreference == Settings.MenuSide.Right;
            float anchorX = isRight ? 1f : 0f;
            rt.anchorMin = new Vector2(anchorX, 0.5f);
            rt.anchorMax = new Vector2(anchorX, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = GetOffscreenPosition();
        }

        [Inject]
        public void Init(IStateMachine stateMachine, ITelemetryService telemetryService)
        {
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;
        }

        public void OnEnable()
        {
            this.telemetryService.LogInfo("Enabling SingleRoundMenuController");

            this.gameBoard = GameObject.Find(Constants.GameObjects.GameBoard).GetComponent<GameBoard>();
            this.singleRoundMenuContainer = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SingleRoundMenuContainer);
            this.ApplyMenuSide();

            var mainMenuGO = GameObjectExtensions.FindGameObjectByName("MainMenu");
            this.mainMenuController = mainMenuGO.GetComponent<MainMenuController>();

            this.heartsButton = GetButtonObject("HeartsButton");
            this.queensButton = GetButtonObject("QueensButton");
            this.kingOfHeartsButton = GetButtonObject("KingOfHeartsButton");
            this.pilesButton = GetButtonObject("PilesButton");
            this.nothingButton = GetButtonObject("NothingButton");
            this.everythingButton = GetButtonObject("EverythingButton");

            this.heartsButton.onClick.AddListener(this.HandleHeartsButtonClick);
            this.queensButton.onClick.AddListener(this.HandleQueensButtonClick);
            this.kingOfHeartsButton.onClick.AddListener(this.HandleKingOfHeartsButtonClick);
            this.pilesButton.onClick.AddListener(this.HandlePilesButtonClick);
            this.nothingButton.onClick.AddListener(this.HandleNothingButtonClick);
            this.everythingButton.onClick.AddListener(this.HandleEverythingButtonClick);
        }

        public void OnDisable()
        {
            this.telemetryService.LogInfo("Disabling SingleRoundMenuController");
            this.heartsButton.onClick.RemoveAllListeners();
            this.queensButton.onClick.RemoveAllListeners();
            this.kingOfHeartsButton.onClick.RemoveAllListeners();
            this.pilesButton.onClick.RemoveAllListeners();
            this.nothingButton.onClick.RemoveAllListeners();
            this.everythingButton.onClick.RemoveAllListeners();
        }

        public void ToggleMenuVisibility()
        {
            if (this.IsExtended())
            {
                this.telemetryService.LogInfo("Moving single round menu back behind main menu");
                this.stateMachine.SetMenuOpen(false);
                StartCoroutine(this.MoveMenu(this.GetBehindMenuPosition(), menuSlideSpeed));
            }
            else
            {
                this.telemetryService.LogInfo("Moving single round menu into view");
                this.stateMachine.SetMenuOpen(true);
                StartCoroutine(this.MoveMenu(this.GetInViewPosition(), menuSlideSpeed));
            }
        }

        public void FollowMenuIn()
        {
            this.telemetryService.LogInfo("Single round menu following main menu in");
            StartCoroutine(this.MoveMenu(this.GetBehindMenuPosition(), menuSlideSpeed));
        }

        public void FollowMenuOut()
        {
            this.telemetryService.LogInfo("Single round menu following main menu out");
            this.stateMachine.SetMenuOpen(false);
            var speed = this.IsExtended() ? menuSlideOutSpeedWhenExtended : menuSlideSpeed;
            StartCoroutine(this.MoveMenu(this.GetOffscreenPosition(), speed));
        }

        public bool IsExtended()
        {
            var rt = this.singleRoundMenuContainer.GetComponent<RectTransform>();
            return Settings.MenuSidePreference == Settings.MenuSide.Right
                ? rt.anchoredPosition.x <= this.GetBehindMenuPosition().x - 1f
                : rt.anchoredPosition.x >= this.GetBehindMenuPosition().x + 1f;
        }

        private void HandleHeartsButtonClick()
        {
            this.telemetryService.LogInfo("Hearts button clicked");
            this.FollowMenuOut();
            this.mainMenuController.HideMenu();
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Hearts);
        }

        private void HandleQueensButtonClick()
        {
            this.telemetryService.LogInfo("Queens button clicked");
            this.FollowMenuOut();
            this.mainMenuController.HideMenu();
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Queens);
        }

        private void HandleKingOfHeartsButtonClick()
        {
            this.telemetryService.LogInfo("King of Hearts button clicked");
            this.FollowMenuOut();
            this.mainMenuController.HideMenu();
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.KingOfHearts);
        }

        private void HandlePilesButtonClick()
        {
            this.telemetryService.LogInfo("Piles button clicked");
            this.FollowMenuOut();
            this.mainMenuController.HideMenu();
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Piles);
        }

        private void HandleNothingButtonClick()
        {
            this.telemetryService.LogInfo("Nothing button clicked");
            this.FollowMenuOut();
            this.mainMenuController.HideMenu();
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Nothing);
        }

        private void HandleEverythingButtonClick()
        {
            this.telemetryService.LogInfo("Everything button clicked");
            this.FollowMenuOut();
            this.mainMenuController.HideMenu();
            this.gameBoard.CreateNewGame(
                Constants.SingleRoundManager.GameName,
                Constants.SingleRoundManager.Everything);
        }

        private Button GetButtonObject(string name)
        {
            var buttonObject = GameObjectExtensions.FindGameObjectByName(name);
            return buttonObject.GetComponent<Button>();
        }

        private IEnumerator MoveMenu(Vector2 finalPosition, float speed)
        {
            var baseMoveSpeed = speed;
            var rectTransform = this.singleRoundMenuContainer.GetComponent<RectTransform>();
            while (rectTransform.anchoredPosition != finalPosition)
            {
                rectTransform.anchoredPosition = Vector2.MoveTowards(
                    rectTransform.anchoredPosition,
                    finalPosition,
                    baseMoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
