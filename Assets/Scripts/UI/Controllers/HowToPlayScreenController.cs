namespace Barbu.UI.Controllers
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class HowToPlayScreenController : MonoBehaviour
    {
        private GameObject howToPlayScreen;
        private Button closeBtn;

        public void OnEnable()
        {
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
            Debug.Log("Closing how to play screen");
            this.howToPlayScreen.SetActive(false);
        }
    }
}
