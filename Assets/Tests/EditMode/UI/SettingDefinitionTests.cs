namespace Barbu.Tests.EditMode.UI
{
    using System;
    using System.Collections.Generic;
    using Barbu.UI.SettingsMenu;
    using NUnit.Framework;

    public class SettingDefinitionTests
    {
        private enum Fruit
        {
            Apple = 0,
            Pear = 1,
            Plum = 2,
        }

        [Test]
        public void Step_MovesToTheNextOption()
        {
            var selected = 0;
            var definition = new SettingDefinition(
                "Example",
                SettingControlKind.Selector,
                new[] { "A", "B", "C" },
                () => selected,
                index => selected = index);

            definition.Step(1);

            Assert.AreEqual(1, selected);
            Assert.AreEqual("B", definition.DisplayValue);
        }

        [Test]
        public void Step_WrapsPastTheLastOption()
        {
            var selected = 2;
            var definition = new SettingDefinition(
                "Example",
                SettingControlKind.Selector,
                new[] { "A", "B", "C" },
                () => selected,
                index => selected = index);

            definition.Step(1);

            Assert.AreEqual(0, selected);
        }

        // The case a plain % gets wrong: it would leave the index at -1.
        [Test]
        public void Step_WrapsBackwardsPastTheFirstOption()
        {
            var selected = 0;
            var definition = new SettingDefinition(
                "Example",
                SettingControlKind.Selector,
                new[] { "A", "B", "C" },
                () => selected,
                index => selected = index);

            definition.Step(-1);

            Assert.AreEqual(2, selected);
        }

        [Test]
        public void SelectedIndex_OutOfRangeStoredValue_ReadsAsTheFirstOption()
        {
            // A preference saved before an option was removed from the game.
            var selected = 7;
            var definition = new SettingDefinition(
                "Example",
                SettingControlKind.Selector,
                new[] { "A", "B" },
                () => selected,
                index => selected = index);

            Assert.AreEqual(0, definition.SelectedIndex);
            Assert.AreEqual("A", definition.DisplayValue);
        }

        [Test]
        public void Constructor_WithoutOptions_Throws()
        {
            Assert.Throws<ArgumentException>(() => new SettingDefinition(
                "Example",
                SettingControlKind.Selector,
                new string[0],
                () => 0,
                _ => { }));
        }

        [Test]
        public void Constructor_WithoutLabel_Throws()
        {
            Assert.Throws<ArgumentException>(() => new SettingDefinition(
                string.Empty,
                SettingControlKind.Selector,
                new[] { "A" },
                () => 0,
                _ => { }));
        }

        [Test]
        public void ForEnum_TakesItsOptionsFromTheEnumMembers()
        {
            var value = Fruit.Apple;
            var definition = SettingDefinition.ForEnum("Fruit", () => value, v => value = v);

            CollectionAssert.AreEqual(new[] { "Apple", "Pear", "Plum" }, definition.OptionLabels);
        }

        [Test]
        public void ForEnum_UsesDisplayNamesWhereGiven()
        {
            var value = Fruit.Apple;
            var displayNames = new Dictionary<Fruit, string> { [Fruit.Plum] = "Damson" };

            var definition = SettingDefinition.ForEnum("Fruit", () => value, v => value = v, displayNames);

            // Members left out of the table keep their own name.
            CollectionAssert.AreEqual(new[] { "Apple", "Pear", "Damson" }, definition.OptionLabels);
        }

        [Test]
        public void ForEnum_StepWritesTheEnumValueBack()
        {
            var value = Fruit.Pear;
            var definition = SettingDefinition.ForEnum("Fruit", () => value, v => value = v);

            definition.Step(1);

            Assert.AreEqual(Fruit.Plum, value);
        }

        [Test]
        public void ForBool_StepFlipsTheStoredValue()
        {
            var value = false;
            var definition = SettingDefinition.ForBool("Enabled", () => value, v => value = v);

            Assert.AreEqual(SettingControlKind.Toggle, definition.ControlKind);
            Assert.AreEqual("Off", definition.DisplayValue);

            definition.Step(1);

            Assert.IsTrue(value);
            Assert.AreEqual("On", definition.DisplayValue);
        }

        [Test]
        public void ForBool_DisplayValueOverride_ReplacesTheOptionLabel()
        {
            var value = true;
            var definition = SettingDefinition.ForBool(
                "Enabled",
                () => value,
                v => value = v,
                getDisplayValue: () => "On *");

            Assert.AreEqual("On *", definition.DisplayValue);
        }
    }
}
