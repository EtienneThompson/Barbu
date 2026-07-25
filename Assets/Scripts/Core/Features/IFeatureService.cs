namespace Barbu.Core.Features
{
    using System.Collections.Generic;

    /// <summary>
    /// Answers whether a feature flag is currently on, and lets development builds
    /// override that answer at runtime.
    /// </summary>
    public interface IFeatureService
    {
        /// <summary>Every registered flag, in registration order. Used to build the debug panel.</summary>
        IReadOnlyList<FeatureDefinition> Definitions { get; }

        /// <summary>
        /// Whether the feature is on right now. Unregistered names resolve to off.
        /// </summary>
        bool IsEnabled(string feature);

        /// <summary>Whether the feature has been explicitly forced on or off.</summary>
        bool HasOverride(string feature);

        void SetOverride(string feature, bool enabled);

        void ClearOverride(string feature);

        void ClearAllOverrides();
    }
}
