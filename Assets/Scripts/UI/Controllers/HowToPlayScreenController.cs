namespace Barbu.UI.Controllers
{
    using Barbu.Core;
    using Barbu.Interfaces.Core;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class HowToPlayScreenController : MonoBehaviour
    {
        private ITelemetryService telemetryService;
        private GameObject howToPlayScreen;
        private Button closeBtn;

        public void OnEnable()
        {
            this.telemetryService = TelemetryService.GetInstance();
            this.howToPlayScreen = GameObject.Find(Constants.GameObjects.HowToPlayScreen);
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
