namespace Barbu.UI.SettingsMenu
{
    using System.Collections.Generic;
    using Barbu.UI.Controllers;

    // Barbu.Settings would otherwise be shadowed by this namespace's own last segment.
    using GameSettings = Barbu.Settings;

    /// <summary>
    /// The single place every player facing setting is declared. The settings menu renders
    /// this list in order, one row per entry.
    /// </summary>
    /// <remarks>
    /// To add a setting: give it storage on <see cref="Barbu.Settings"/>, then add one entry
    /// here. Nothing in the menu itself needs to change. Pass requiredFeature to keep a row
    /// hidden until its flag is on, and developmentOnly to keep it out of release builds
    /// entirely.
    /// </remarks>
    public static class SettingsRegistry
    {
        /// <summary>
        /// The suit pips and arrows in the hand sorting options are outside the default font's
        /// coverage, so that row draws its value in a font that has them.
        /// </summary>
        private const string SuitGlyphFont = "Fonts/LucidaSansUnicodeRegular";

        private static readonly IReadOnlyDictionary<GameSettings.SortingOptions, string> HandSortingLabels =
            new Dictionary<GameSettings.SortingOptions, string>
            {
                [GameSettings.SortingOptions.None] = "No Sorting",
                [GameSettings.SortingOptions.LowToHigh] = "2 → A",
                [GameSettings.SortingOptions.HighToLow] = "A → 2",
                [GameSettings.SortingOptions.SuitLowToHigh] = "♥ ♦ ♠ ♣ 2 → A",
                [GameSettings.SortingOptions.SuitHighToLow] = "♥ ♦ ♠ ♣ A → 2",
                [GameSettings.SortingOptions.SuitLowToHighAlternating] = "♥ ♠ ♦ ♣ 2 → A",
                [GameSettings.SortingOptions.SuitHighToLowAlternating] = "♥ ♠ ♦ ♣ A → 2",
            };

        /// <summary>
        /// Every setting the menu can show, in the order it shows them.
        /// </summary>
        public static readonly IReadOnlyList<SettingDefinition> All = new[]
        {
            SettingDefinition.ForEnum(
                "Hand Sorting",
                () => GameSettings.SortingPreference,
                value => GameSettings.SortingPreference = value,
                HandSortingLabels,
                fontResourcePath: SuitGlyphFont),

            SettingDefinition.ForEnum(
                "Computer Difficulty",
                () => GameSettings.ComputerDifficultyPreference,
                value => GameSettings.ComputerDifficultyPreference = value),

            SettingDefinition.ForEnum(
                "Card Back Color",
                () => GameSettings.BackColorPreference,
                value => GameSettings.BackColorPreference = value),

            SettingDefinition.ForEnum(
                "Menu Side",
                () => GameSettings.MenuSidePreference,
                value =>
                {
                    GameSettings.MenuSidePreference = value;
                    NotifyMenuSideChanged();
                }),
        };

        /// <summary>
        /// Moves the menus that are already on screen to the newly chosen side, since they
        /// only read the preference when they are enabled.
        /// </summary>
        private static void NotifyMenuSideChanged()
        {
            var mainMenu = GameObjectExtensions.FindGameObjectByName("MainMenu");
            if (mainMenu != null)
            {
                var mainMenuController = mainMenu.GetComponent<MainMenuController>();
                if (mainMenuController != null)
                {
                    mainMenuController.ApplyMenuSide();
                }
            }

            var singleRoundMenu = GameObjectExtensions.FindGameObjectByName(
                Constants.GameObjects.SingleRoundMenu,
                findInactive: true);
            if (singleRoundMenu != null)
            {
                var singleRoundMenuController = singleRoundMenu.GetComponent<SingleRoundMenuController>();
                if (singleRoundMenuController != null)
                {
                    singleRoundMenuController.ApplyMenuSide();
                }
            }
        }
    }
}
