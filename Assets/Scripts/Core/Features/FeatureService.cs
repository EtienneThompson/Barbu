namespace Barbu.Core.Features
{
    using System.Collections.Generic;

    /// <summary>
    /// Resolves feature flags from, in order of priority: an explicit override, then
    /// the flag's default for the current build type.
    /// </summary>
    public class FeatureService : IFeatureService
    {
        private readonly IFeatureOverrideStore store;
        private readonly bool isDevelopmentBuild;

        /// <param name="definitions">Normally FeatureRegistry.All.</param>
        /// <param name="isDevelopmentBuild">
        /// True in the editor and in development builds. Selects which default each flag
        /// falls back to, and gates whether overrides are honoured at all.
        /// </param>
        public FeatureService(
            IReadOnlyList<FeatureDefinition> definitions,
            IFeatureOverrideStore store,
            bool isDevelopmentBuild)
        {
            this.Definitions = definitions;
            this.store = store;
            this.isDevelopmentBuild = isDevelopmentBuild;
        }

        public IReadOnlyList<FeatureDefinition> Definitions { get; }

        public bool IsEnabled(string feature)
        {
            if (!this.TryGetDefinition(feature, out var definition))
            {
                // An unregistered flag is off. Flags are referenced through the constants on
                // FeatureRegistry, so this is reachable mainly by a flag that has already
                // been retired while a call site still lingers, and off is the safe answer.
                return false;
            }

            // Overrides exist purely to support iteration in the editor and in development
            // builds. A release build always resolves to the shipped default, so a hand
            // edited PlayerPrefs file on a player's device cannot switch on unfinished work.
            if (this.isDevelopmentBuild && this.store.TryGetOverride(feature, out var overridden))
            {
                return overridden;
            }

            return definition.DefaultFor(this.isDevelopmentBuild);
        }

        public bool HasOverride(string feature)
        {
            return this.store.TryGetOverride(feature, out _);
        }

        public void SetOverride(string feature, bool enabled)
        {
            this.store.SetOverride(feature, enabled);
        }

        public void ClearOverride(string feature)
        {
            this.store.ClearOverride(feature);
        }

        public void ClearAllOverrides()
        {
            for (int i = 0; i < this.Definitions.Count; i++)
            {
                this.store.ClearOverride(this.Definitions[i].Name);
            }
        }

        private bool TryGetDefinition(string feature, out FeatureDefinition definition)
        {
            for (int i = 0; i < this.Definitions.Count; i++)
            {
                if (this.Definitions[i].Name == feature)
                {
                    definition = this.Definitions[i];
                    return true;
                }
            }

            definition = default;
            return false;
        }
    }
}
