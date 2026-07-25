namespace Barbu.Core.Features
{
    using UnityEngine;

    /// <summary>
    /// Stores feature flag overrides in PlayerPrefs, one key per flag.
    /// </summary>
    /// <remarks>
    /// PlayerPrefs.GetInt returns 0 for a missing key, which is indistinguishable from a
    /// stored "off", so HasKey is what actually distinguishes unset from forced off.
    /// </remarks>
    public class PlayerPrefsFeatureOverrideStore : IFeatureOverrideStore
    {
        public bool TryGetOverride(string feature, out bool enabled)
        {
            var key = KeyFor(feature);
            if (!PlayerPrefs.HasKey(key))
            {
                enabled = false;
                return false;
            }

            enabled = PlayerPrefs.GetInt(key) == 1;
            return true;
        }

        public void SetOverride(string feature, bool enabled)
        {
            PlayerPrefs.SetInt(KeyFor(feature), enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ClearOverride(string feature)
        {
            PlayerPrefs.DeleteKey(KeyFor(feature));
            PlayerPrefs.Save();
        }

        private static string KeyFor(string feature)
        {
            return Constants.PlayerPrefsKeys.FeatureOverridePrefix + feature;
        }
    }
}
