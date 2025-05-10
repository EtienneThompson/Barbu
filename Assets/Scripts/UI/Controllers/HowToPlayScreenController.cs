namespace Barbu.UI.Controllers
{
    using Barbu.Core;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Zenject;

    public class HowToPlayScreenController : MonoBehaviour
    {
        private ITelemetryService telemetryService;
        private GameObject howToPlayScreen;
        private Button closeBtn;

        [Inject]
        public void Init(ITelemetryService telemetryService)
        {
            this.telemetryService = telemetryService;
        }

        public void OnEnable()
        {
            this.howToPlayScreen = GameObjectExtensions.FindGameObjectByName(Constants.GameObjects.HowToPlayScreen, findInactive: true);
            var document = this.howToPlayScreen.GetComponent<UIDocument>();
            var root = document.rootVisualElement;

            this.closeBtn = root.Q<Button>("close");

            this.closeBtn.RegisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        public void OnDisable()
        {
            this.closeBtn.UnregisterCallback<ClickEvent>(HandleCloseButtonClick);
        }

        private void HandleCloseButtonClick(ClickEvent evt)
        {
            this.telemetryService.LogInfo("Closing how to play screen");
            this.howToPlayScreen.SetActive(false);
        }
    }
}
