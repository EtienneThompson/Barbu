namespace Barbu.UI.Components
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A settings row drawn as one button showing the current value, which advances to the
    /// next option when pressed.
    /// </summary>
    /// <remarks>
    /// Intended for two option settings, but it steps rather than inverts, so a definition
    /// with more options still cycles correctly instead of getting stuck.
    /// </remarks>
    public class ToggleSettingRowView : SettingRowView
    {
        [SerializeField]
        private Button toggleButton;

        protected override void OnBind()
        {
            if (this.toggleButton != null)
            {
                this.toggleButton.onClick.RemoveAllListeners();
                this.toggleButton.onClick.AddListener(() => this.Step(1));
            }
        }
    }
}
