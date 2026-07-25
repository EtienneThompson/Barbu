namespace Barbu.Tests.EditMode.TestUtils
{
    using System.Collections.Generic;
    using Barbu.Core.Features;

    /// <summary>
    /// In memory IFeatureOverrideStore so feature flag tests do not read or write the real
    /// PlayerPrefs, which on Windows is the actual registry shared with the running game.
    /// </summary>
    public class FakeFeatureOverrideStore : IFeatureOverrideStore
    {
        public readonly Dictionary<string, bool> Overrides = new();

        public bool TryGetOverride(string feature, out bool enabled)
        {
            return this.Overrides.TryGetValue(feature, out enabled);
        }

        public void SetOverride(string feature, bool enabled)
        {
            this.Overrides[feature] = enabled;
        }

        public void ClearOverride(string feature)
        {
            this.Overrides.Remove(feature);
        }
    }
}
