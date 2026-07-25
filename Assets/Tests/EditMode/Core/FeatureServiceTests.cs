namespace Barbu.Tests.EditMode.Core
{
    using System.Collections.Generic;
    using Barbu.Core.Features;
    using Barbu.Tests.EditMode.TestUtils;
    using NUnit.Framework;

    public class FeatureServiceTests
    {
        private const string InDevelopment = "InDevelopment";
        private const string Shipped = "Shipped";

        // Deliberately not FeatureRegistry.All: these tests are about how a flag resolves,
        // not about which flags the game currently declares, so they must not start failing
        // the day a real flag is added or retired.
        private static readonly IReadOnlyList<FeatureDefinition> Definitions = new FeatureDefinition[]
        {
            new FeatureDefinition(InDevelopment, defaultInDevelopment: true, defaultInRelease: false),
            new FeatureDefinition(Shipped, defaultInDevelopment: true, defaultInRelease: true),
        };

        private FakeFeatureOverrideStore store;

        [SetUp]
        public void SetUp()
        {
            this.store = new FakeFeatureOverrideStore();
        }

        private FeatureService CreateService(bool isDevelopmentBuild)
        {
            return new FeatureService(Definitions, this.store, isDevelopmentBuild);
        }

        [Test]
        public void IsEnabled_NoOverride_UsesDevelopmentDefault()
        {
            var service = this.CreateService(isDevelopmentBuild: true);

            Assert.IsTrue(service.IsEnabled(InDevelopment));
            Assert.IsTrue(service.IsEnabled(Shipped));
        }

        [Test]
        public void IsEnabled_NoOverride_UsesReleaseDefault()
        {
            var service = this.CreateService(isDevelopmentBuild: false);

            Assert.IsFalse(service.IsEnabled(InDevelopment));
            Assert.IsTrue(service.IsEnabled(Shipped));
        }

        [Test]
        public void IsEnabled_OverriddenOn_ReturnsTrueDespiteDefaultOff()
        {
            // A flag that is off even in a development build, so the override is doing all
            // of the work rather than agreeing with the default.
            var service = new FeatureService(
                new FeatureDefinition[]
                {
                    new FeatureDefinition(InDevelopment, defaultInDevelopment: false, defaultInRelease: false),
                },
                this.store,
                isDevelopmentBuild: true);
            Assert.IsFalse(service.IsEnabled(InDevelopment));

            service.SetOverride(InDevelopment, true);

            Assert.IsTrue(service.IsEnabled(InDevelopment));
        }

        // The case the three state store exists for: unset and forced off both store "false"
        // in a two state design, so a flag defaulting on could never be switched off.
        [Test]
        public void IsEnabled_OverriddenOff_ReturnsFalseDespiteDefaultOn()
        {
            var service = this.CreateService(isDevelopmentBuild: true);

            service.SetOverride(InDevelopment, false);

            Assert.IsFalse(service.IsEnabled(InDevelopment));
        }

        [Test]
        public void IsEnabled_ReleaseBuild_IgnoresOverrides()
        {
            var service = this.CreateService(isDevelopmentBuild: false);

            service.SetOverride(InDevelopment, true);
            service.SetOverride(Shipped, false);

            Assert.IsFalse(service.IsEnabled(InDevelopment));
            Assert.IsTrue(service.IsEnabled(Shipped));
        }

        [Test]
        public void IsEnabled_UnregisteredFeature_ReturnsFalse()
        {
            var service = this.CreateService(isDevelopmentBuild: true);

            Assert.IsFalse(service.IsEnabled("NeverRegistered"));
        }

        [Test]
        public void IsEnabled_UnregisteredFeatureWithOverride_StillReturnsFalse()
        {
            var service = this.CreateService(isDevelopmentBuild: true);

            service.SetOverride("NeverRegistered", true);

            Assert.IsFalse(service.IsEnabled("NeverRegistered"));
        }

        [Test]
        public void HasOverride_TracksWhetherAValueWasForced()
        {
            var service = this.CreateService(isDevelopmentBuild: true);

            Assert.IsFalse(service.HasOverride(InDevelopment));

            // Forcing a flag to the value it already had still counts as an override.
            service.SetOverride(InDevelopment, true);

            Assert.IsTrue(service.HasOverride(InDevelopment));
        }

        [Test]
        public void ClearOverride_RestoresTheBuildDefault()
        {
            var service = this.CreateService(isDevelopmentBuild: true);
            service.SetOverride(InDevelopment, false);

            service.ClearOverride(InDevelopment);

            Assert.IsFalse(service.HasOverride(InDevelopment));
            Assert.IsTrue(service.IsEnabled(InDevelopment));
        }

        [Test]
        public void ClearAllOverrides_RestoresEveryRegisteredFlag()
        {
            var service = this.CreateService(isDevelopmentBuild: true);
            service.SetOverride(InDevelopment, false);
            service.SetOverride(Shipped, false);

            service.ClearAllOverrides();

            Assert.IsFalse(service.HasOverride(InDevelopment));
            Assert.IsFalse(service.HasOverride(Shipped));
            Assert.IsTrue(service.IsEnabled(InDevelopment));
            Assert.IsTrue(service.IsEnabled(Shipped));
        }

        [Test]
        public void Definitions_ExposesRegistrationOrder()
        {
            var service = this.CreateService(isDevelopmentBuild: true);

            Assert.AreEqual(2, service.Definitions.Count);
            Assert.AreEqual(InDevelopment, service.Definitions[0].Name);
            Assert.AreEqual(Shipped, service.Definitions[1].Name);
        }

        [Test]
        public void DefaultFor_SelectsDefaultByBuildType()
        {
            var definition = new FeatureDefinition("Example", defaultInDevelopment: true, defaultInRelease: false);

            Assert.IsTrue(definition.DefaultFor(isDevelopmentBuild: true));
            Assert.IsFalse(definition.DefaultFor(isDevelopmentBuild: false));
        }
    }
}
