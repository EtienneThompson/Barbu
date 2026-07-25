namespace Barbu.Core.Features
{
    /// <summary>
    /// Persistence for manually set feature flag overrides.
    /// </summary>
    /// <remarks>
    /// This is deliberately a three state store rather than a bool: a flag can be
    /// unset (no override, so the build's default applies), forced on, or forced off.
    /// Collapsing "unset" and "forced off" together would make it impossible to turn
    /// off a flag whose default is on, which is exactly the case for a feature that is
    /// currently being worked on.
    /// </remarks>
    public interface IFeatureOverrideStore
    {
        /// <summary>
        /// Reads the override for a feature.
        /// </summary>
        /// <returns>True if an override is set, in which case enabled holds its value.</returns>
        bool TryGetOverride(string feature, out bool enabled);

        void SetOverride(string feature, bool enabled);

        /// <summary>Removes any override, returning the feature to its build default.</summary>
        void ClearOverride(string feature);
    }
}
