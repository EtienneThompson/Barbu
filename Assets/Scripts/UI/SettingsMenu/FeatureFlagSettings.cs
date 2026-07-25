#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Barbu.UI.SettingsMenu
{
    using System.Collections.Generic;
    using Barbu.Core.Features;

    /// <summary>
    /// Turns the live feature flag registry into settings rows, so flags can be toggled on a
    /// device without a rebuild.
    /// </summary>
    /// <remarks>
    /// Compiled only into the editor and development builds. These are built here rather than
    /// declared in <see cref="SettingsRegistry"/> because the flag list is not fixed: it comes
    /// from whatever <see cref="IFeatureService"/> currently knows about.
    /// </remarks>
    public static class FeatureFlagSettings
    {
        public static IReadOnlyList<SettingDefinition> CreateDefinitions(IFeatureService featureService)
        {
            var definitions = new List<SettingDefinition>(featureService.Definitions.Count);
            foreach (var feature in featureService.Definitions)
            {
                definitions.Add(CreateDefinition(featureService, feature.Name));
            }

            return definitions;
        }

        private static SettingDefinition CreateDefinition(IFeatureService featureService, string feature)
        {
            return SettingDefinition.ForBool(
                feature,
                () => featureService.IsEnabled(feature),
                enabled => featureService.SetOverride(feature, enabled),
                developmentOnly: true,

                // A flag can gate another row, so the menu has to be rebuilt after one changes.
                affectsMenuLayout: true,

                // The asterisk marks a flag that has been forced either way, to distinguish it
                // from one still sitting at whatever this build type defaults to.
                getDisplayValue: () =>
                {
                    var state = featureService.IsEnabled(feature) ? "On" : "Off";
                    return featureService.HasOverride(feature) ? state + " *" : state;
                });
        }
    }
}
#endif
