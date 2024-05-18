namespace Barbu.UI.Controllers
{
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using Barbu.Models;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MenuButtonsController : MonoBehaviour
    {
        private StateMachine stateMachine;
        private EventsController eventsController;
        private GlobalContext globalContext;
        private ITelemetryService telemetryService;

        private GameObject inGamePoints;
        private GameObject roundOverlay;
        private GameObject menuButtons;
        private GameObject gamesMenu;
        private GameObject settingsMenu;
        private GameObject howToPlayScreen;
        private ScoreMenuController scoreMenuController;

        private Button settingsBtn;
        private Button gamesBtn;
        private Button howToPlayBtn;
        private Button scoreBtn;

        public void OnEnable()
        {
            this.stateMachine = new StateMachine();
            this.eventsController = EventsController.GetInstance();
            this.globalContext = GlobalContext.GetInstance();
            this.telemetryService = TelemetryService.GetInstance();

            this.inGamePoints = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.InGamePoints, findInactive: true);
            this.roundOverlay = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.RoundOverlay, findInactive: true);
            this.gamesMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.GamesMenu, findInactive: true);
            this.settingsMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SettingsMenu, findInactive: true);
            this.menuButtons = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.InGamePoints, findInactive: true);
            this.howToPlayScreen = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.HowToPlayScreen, findInactive: true);
            var scoreCanvas = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.ScoreMenuCanvas, findInactive: true);
            this.scoreMenuController = scoreCanvas.GetComponent<ScoreMenuController>();
            var document = this.menuButtons.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            this.settingsBtn = root.Q<Button>("settings");
            this.gamesBtn = root.Q<Button>("games");
            this.howToPlayBtn = root.Q<Button>("howtoplay");
            this.scoreBtn = root.Q<Button>("score");

            this.settingsBtn.RegisterCallback<ClickEvent>((e) => HandleSettingsButtonClick(e));
            this.gamesBtn.RegisterCallback<ClickEvent>(HandleGamesButtonClick);
            this.howToPlayBtn.RegisterCallback<ClickEvent>(HandleHowToPlayButtonClick);
            this.scoreBtn.RegisterCallback<ClickEvent>(HandleScoreButtonClick);
        }

        public void OnDisable()
        {
            this.settingsBtn.UnregisterCallback<ClickEvent>(HandleSettingsButtonClick);
            this.gamesBtn.UnregisterCallback<ClickEvent>(HandleGamesButtonClick);
            this.howToPlayBtn.UnregisterCallback<ClickEvent>(HandleHowToPlayButtonClick);
            this.scoreBtn.UnregisterCallback<ClickEvent>(HandleScoreButtonClick);
        }

        private void HandleSettingsButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Settings button clicked");
            this.settingsMenu.SetActive(true);
        }

        private void HandleGamesButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Handling games button click");
            this.gamesMenu.SetActive(true);
        }

        private void HandleHowToPlayButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Handling how to play button click");
            this.howToPlayScreen.SetActive(true);
        }

        private void HandleScoreButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Handling score button click");
            this.stateMachine.SetMenuOpen(true);
            this.eventsController.Fire(EventNames.PauseGame);
            this.inGamePoints.SetActive(false);
            this.roundOverlay.SetActive(false);
            this.scoreMenuController.DisplayScores(
                this.globalContext.RoundWorkflow.GetPlayerPoints(),
                this.globalContext.RoundWorkflow.GetCurrentRoundIndex(),
                skipAnimations: true);
        }
    }
}
