namespace Barbu.UI.Components
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A settings row whose value sits between a previous and a next button, each stepping one
    /// option and wrapping at the ends.
    /// </summary>
    public class SelectorSettingRowView : SettingRowView
    {
        [SerializeField]
        private Button previousButton;

        [SerializeField]
        private Button nextButton;

        protected override void OnBind()
        {
            if (this.previousButton != null)
            {
                this.previousButton.onClick.RemoveAllListeners();
                this.previousButton.onClick.AddListener(() => this.Step(-1));
            }

            if (this.nextButton != null)
            {
                this.nextButton.onClick.RemoveAllListeners();
                this.nextButton.onClick.AddListener(() => this.Step(1));
            }
        }
    }
}
