namespace Barbu.Tests.EditMode.UI
{
    using System.Linq;
    using Barbu.UI.SettingsMenu;
    using NUnit.Framework;

    /// <summary>
    /// Guards the shape of the settings list rather than its contents, so adding a setting does
    /// not mean editing these tests, but adding a broken one fails them.
    /// </summary>
    public class SettingsRegistryTests
    {
        [Test]
        public void All_IsNotEmpty()
        {
            Assert.IsNotEmpty(SettingsRegistry.All);
        }

        [Test]
        public void All_EverySettingHasALabel()
        {
            foreach (var definition in SettingsRegistry.All)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(definition.Label));
            }
        }

        // Labels are how a row is identified in the telemetry log, and two rows reading the same
        // is a sign of a copy pasted entry.
        [Test]
        public void All_LabelsAreUnique()
        {
            var labels = SettingsRegistry.All.Select(definition => definition.Label).ToList();

            CollectionAssert.AllItemsAreUnique(labels);
        }

        [Test]
        public void All_EverySettingHasAtLeastTwoOptions()
        {
            foreach (var definition in SettingsRegistry.All)
            {
                Assert.GreaterOrEqual(
                    definition.OptionLabels.Count,
                    2,
                    $"'{definition.Label}' has nothing to switch between.");
            }
        }

        [Test]
        public void All_NoOptionLabelIsBlank()
        {
            foreach (var definition in SettingsRegistry.All)
            {
                foreach (var option in definition.OptionLabels)
                {
                    Assert.IsFalse(
                        string.IsNullOrWhiteSpace(option),
                        $"'{definition.Label}' has a blank option.");
                }
            }
        }

        [Test]
        public void All_SelectedIndexIsInRangeForEverySetting()
        {
            foreach (var definition in SettingsRegistry.All)
            {
                Assert.GreaterOrEqual(definition.SelectedIndex, 0);
                Assert.Less(definition.SelectedIndex, definition.OptionLabels.Count);
                Assert.IsNotNull(definition.DisplayValue);
            }
        }

        // The suit pips only render in a font that has them, so the row that uses them has to
        // ask for one.
        [Test]
        public void HandSorting_OptionsWithSuitGlyphsRequestAFont()
        {
            var handSorting = SettingsRegistry.All.Single(definition => definition.Label == "Hand Sorting");

            Assert.IsTrue(handSorting.OptionLabels.Any(option => option.Contains("♥")));
            Assert.IsFalse(string.IsNullOrEmpty(handSorting.FontResourcePath));
        }

        [Test]
        public void HandSorting_HasOneOptionPerSortingMode()
        {
            var handSorting = SettingsRegistry.All.Single(definition => definition.Label == "Hand Sorting");

            Assert.AreEqual(
                System.Enum.GetValues(typeof(Barbu.Settings.SortingOptions)).Length,
                handSorting.OptionLabels.Count);
        }

        [Test]
        public void Step_RoundTripsThroughStorage()
        {
            var definition = SettingsRegistry.All.Single(d => d.Label == "Computer Difficulty");
            var original = Barbu.Settings.ComputerDifficultyPreference;

            try
            {
                var startingIndex = definition.SelectedIndex;

                definition.Step(1);

                Assert.AreNotEqual(startingIndex, definition.SelectedIndex);

                definition.Step(-1);

                Assert.AreEqual(startingIndex, definition.SelectedIndex);
            }
            finally
            {
                // These read and write the real preferences, so put back what was there.
                Barbu.Settings.ComputerDifficultyPreference = original;
            }
        }
    }
}
