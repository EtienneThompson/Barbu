namespace Barbu.UI.Controllers
{
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MenuButtonsController : MonoBehaviour
    {
        private ITelemetryService telemetryService;
        private GameObject menuButtons;
        private GameObject gamesMenu;
        private GameObject settingsMenu;
        private GameObject howToPlayScreen;
        private Button settingsBtn;
        private Button gamesBtn;
        private Button howToPlayBtn;

        public void OnEnable()
        {
            this.telemetryService = TelemetryService.GetInstance();
            this.gamesMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.GamesMenu, findInactive: true);
            this.settingsMenu = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.SettingsMenu, findInactive: true);
            this.menuButtons = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.InGamePoints, findInactive: true);
            this.howToPlayScreen = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.HowToPlayScreen, findInactive: true);
            var document = this.menuButtons.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            this.settingsBtn = root.Q<Button>("settings");
            this.gamesBtn = root.Q<Button>("games");
            this.howToPlayBtn = root.Q<Button>("howtoplay");

            this.settingsBtn.RegisterCallback<ClickEvent>((e) => HandleSettingsButtonClick(e));
            this.gamesBtn.RegisterCallback<ClickEvent>(HandleGamesButtonClick);
            this.howToPlayBtn.RegisterCallback<ClickEvent>(HandleHowToPlayButtonClick);
        }

        public void OnDisable()
        {
            this.settingsBtn.UnregisterCallback<ClickEvent>(HandleSettingsButtonClick);
            this.gamesBtn.UnregisterCallback<ClickEvent>(HandleGamesButtonClick);
            this.howToPlayBtn.UnregisterCallback<ClickEvent>(HandleHowToPlayButtonClick);
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
    }
}
