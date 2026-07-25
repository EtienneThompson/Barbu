namespace Barbu.UI.SettingsMenu
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// One row of the settings menu: what it is called, which control draws it, the values it
    /// can take, and how to read and write the current one.
    /// </summary>
    /// <remarks>
    /// Every setting is modelled as an index into <see cref="OptionLabels"/> no matter which
    /// control draws it, so a row view only ever has to move an index and display
    /// <see cref="DisplayValue"/>. That is what lets one prefab serve every selector setting.
    /// Prefer the <see cref="ForEnum{TEnum}"/> and <see cref="ForBool"/> factories over the
    /// constructor; the constructor exists for settings that are not backed by either.
    /// </remarks>
    public sealed class SettingDefinition
    {
        private readonly Func<int> getSelectedIndex;
        private readonly Action<int> setSelectedIndex;
        private readonly Func<string> getDisplayValue;

        public SettingDefinition(
            string label,
            SettingControlKind controlKind,
            IReadOnlyList<string> optionLabels,
            Func<int> getSelectedIndex,
            Action<int> setSelectedIndex,
            string requiredFeature = null,
            bool developmentOnly = false,
            bool affectsMenuLayout = false,
            string fontResourcePath = null,
            Func<string> getDisplayValue = null)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentException("A setting needs a label.", nameof(label));
            }

            if (optionLabels == null || optionLabels.Count == 0)
            {
                throw new ArgumentException(
                    $"Setting '{label}' needs at least one option to choose between.",
                    nameof(optionLabels));
            }

            this.Label = label;
            this.ControlKind = controlKind;
            this.OptionLabels = optionLabels;
            this.getSelectedIndex = getSelectedIndex ?? throw new ArgumentNullException(nameof(getSelectedIndex));
            this.setSelectedIndex = setSelectedIndex ?? throw new ArgumentNullException(nameof(setSelectedIndex));
            this.RequiredFeature = requiredFeature;
            this.DevelopmentOnly = developmentOnly;
            this.AffectsMenuLayout = affectsMenuLayout;
            this.FontResourcePath = fontResourcePath;
            this.getDisplayValue = getDisplayValue;
        }

        /// <summary>The name shown on the left of the row.</summary>
        public string Label { get; }

        public SettingControlKind ControlKind { get; }

        /// <summary>The options in selection order, already in the form the player reads.</summary>
        public IReadOnlyList<string> OptionLabels { get; }

        /// <summary>
        /// The feature flag that has to be on for this row to appear, or null for a setting
        /// that is always shown.
        /// </summary>
        public string RequiredFeature { get; }

        /// <summary>Whether this row only appears in the editor and development builds.</summary>
        public bool DevelopmentOnly { get; }

        /// <summary>
        /// Whether changing this setting can change which rows the menu should be showing, and
        /// so requires the menu to be rebuilt. True for the feature flag toggles, because a
        /// flag can gate another row.
        /// </summary>
        public bool AffectsMenuLayout { get; }

        /// <summary>
        /// A font under Resources to draw the value with instead of the prefab's own, for
        /// options whose text needs glyphs the default font has no coverage for. Null to keep
        /// the prefab's font.
        /// </summary>
        public string FontResourcePath { get; }

        /// <summary>
        /// The currently selected option. Anything out of range reads as the first option: a
        /// saved preference can fall outside the list if an option was removed in an update,
        /// and a settings menu is not worth crashing over.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                var index = this.getSelectedIndex();
                return index >= 0 && index < this.OptionLabels.Count ? index : 0;
            }
        }

        /// <summary>The text the row shows for the current value.</summary>
        public string DisplayValue =>
            this.getDisplayValue != null ? this.getDisplayValue() : this.OptionLabels[this.SelectedIndex];

        /// <summary>
        /// Moves the selection by <paramref name="delta"/> options, wrapping around both ends.
        /// </summary>
        public void Step(int delta)
        {
            var count = this.OptionLabels.Count;
            var next = (this.SelectedIndex + delta) % count;
            if (next < 0)
            {
                next += count;
            }

            this.setSelectedIndex(next);
        }

        /// <summary>
        /// Builds a definition for a setting stored as an enum, taking the options from the
        /// enum's members so the option list cannot drift out of sync with the type.
        /// </summary>
        /// <param name="displayNames">
        /// Overrides for members whose name is not what the player should read. Members left
        /// out fall back to their own name.
        /// </param>
        public static SettingDefinition ForEnum<TEnum>(
            string label,
            Func<TEnum> get,
            Action<TEnum> set,
            IReadOnlyDictionary<TEnum, string> displayNames = null,
            SettingControlKind controlKind = SettingControlKind.Selector,
            string requiredFeature = null,
            bool developmentOnly = false,
            string fontResourcePath = null)
            where TEnum : struct, Enum
        {
            if (get == null)
            {
                throw new ArgumentNullException(nameof(get));
            }

            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            var values = (TEnum[])Enum.GetValues(typeof(TEnum));
            var optionLabels = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                optionLabels[i] = displayNames != null && displayNames.TryGetValue(values[i], out var displayName)
                    ? displayName
                    : values[i].ToString();
            }

            return new SettingDefinition(
                label,
                controlKind,
                optionLabels,
                () => Array.IndexOf(values, get()),
                index => set(values[index]),
                requiredFeature,
                developmentOnly,
                fontResourcePath: fontResourcePath);
        }

        /// <summary>
        /// Builds a definition for a setting stored as a bool, drawn as a toggle.
        /// </summary>
        public static SettingDefinition ForBool(
            string label,
            Func<bool> get,
            Action<bool> set,
            string offLabel = "Off",
            string onLabel = "On",
            string requiredFeature = null,
            bool developmentOnly = false,
            bool affectsMenuLayout = false,
            Func<string> getDisplayValue = null)
        {
            if (get == null)
            {
                throw new ArgumentNullException(nameof(get));
            }

            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            return new SettingDefinition(
                label,
                SettingControlKind.Toggle,
                new[] { offLabel, onLabel },
                () => get() ? 1 : 0,
                index => set(index == 1),
                requiredFeature,
                developmentOnly,
                affectsMenuLayout,
                getDisplayValue: getDisplayValue);
        }
    }
}
