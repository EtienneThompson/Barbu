namespace Barbu.Core.Features
{
    /// <summary>
    /// Describes a single feature flag: its stable name, and the value it resolves
    /// to when nobody has explicitly overridden it.
    /// </summary>
    public readonly struct FeatureDefinition
    {
        public FeatureDefinition(string name, bool defaultInDevelopment, bool defaultInRelease)
        {
            this.Name = name;
            this.DefaultInDevelopment = defaultInDevelopment;
            this.DefaultInRelease = defaultInRelease;
        }

        /// <summary>
        /// The stable identifier for the flag. This is also part of the PlayerPrefs key
        /// used to store an override, so renaming it discards any existing override.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether the feature is on by default in the editor and in development builds.
        /// In-development features normally set this to true, so that day to day work
        /// happens with the feature on without anyone having to flip a switch.
        /// </summary>
        public bool DefaultInDevelopment { get; }

        /// <summary>
        /// Whether the feature is on by default in a release build. In-development
        /// features normally set this to false, which is what allows unfinished work to
        /// be merged and shipped without being reachable by players.
        /// </summary>
        public bool DefaultInRelease { get; }

        public bool DefaultFor(bool isDevelopmentBuild)
        {
            return isDevelopmentBuild ? this.DefaultInDevelopment : this.DefaultInRelease;
        }
    }
}
