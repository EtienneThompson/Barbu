namespace Barbu.UI.SettingsMenu
{
    /// <summary>
    /// Which control draws a setting, and therefore which prefab the menu instantiates for it.
    /// </summary>
    /// <remarks>
    /// Adding a value here is a three part change: the value, a prefab under Assets/Prefabs/UI,
    /// and a <see cref="Barbu.UI.Components.SettingRowView"/> subclass on that prefab. Only add
    /// one when a setting actually needs it; a setting that reads well as a selector should be
    /// a selector.
    /// </remarks>
    public enum SettingControlKind
    {
        /// <summary>
        /// A value flanked by two arrow buttons that step backwards and forwards through the
        /// options, wrapping at either end.
        /// </summary>
        Selector = 0,

        /// <summary>
        /// A single button showing the current value, which advances to the next option when
        /// pressed. Intended for two option settings.
        /// </summary>
        Toggle = 1,
    }
}
