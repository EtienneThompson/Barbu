namespace Barbu.Core.Features
{
    using System.Collections.Generic;

    /// <summary>
    /// The single place every feature flag is declared.
    /// </summary>
    /// <remarks>
    /// To add a flag, declare its name as a constant and register a definition for it.
    /// A feature that is under construction wants defaultInDevelopment: true and
    /// defaultInRelease: false, so that it is on while being worked on and invisible to
    /// players. When the feature is finished, flip defaultInRelease to true, ship it,
    /// and then delete the flag and its call sites entirely so flags do not accumulate.
    ///
    /// Nothing is registered yet, so <see cref="All"/> is intentionally empty.
    /// </remarks>
    public static class FeatureRegistry
    {
        // Declare each flag's name here, for example:
        // public const string NewScoring = "NewScoring";

        /// <summary>
        /// Every flag known to the game. Anything not in this list resolves to off.
        /// </summary>
        public static readonly IReadOnlyList<FeatureDefinition> All = new FeatureDefinition[]
        {
            // ...then register it here, for example:
            // new FeatureDefinition(NewScoring, defaultInDevelopment: true, defaultInRelease: false),
        };
    }
}
