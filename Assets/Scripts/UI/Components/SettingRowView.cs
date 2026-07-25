namespace Barbu.UI.Components
{
    using System;
    using System.Collections.Generic;
    using Barbu.UI.SettingsMenu;
    using TMPro;
    using UnityEngine;

    /// <summary>
    /// The behaviour shared by every settings row prefab: show a definition's label and
    /// current value, and report when the player changes it.
    /// </summary>
    /// <remarks>
    /// A row knows nothing about <see cref="Barbu.Settings"/> or PlayerPrefs. It moves an
    /// index on the definition it was bound to and redraws, which is what keeps one prefab
    /// usable by every setting of its kind.
    /// </remarks>
    public abstract class SettingRowView : MonoBehaviour
    {
        /// <summary>
        /// Fonts built from a <see cref="Font"/> at runtime, kept for the life of the process.
        /// Building one is not cheap and the same font is wanted every time the menu opens.
        /// Failures are cached too, so a missing font is looked up once rather than per row.
        /// </summary>
        private static readonly Dictionary<string, TMP_FontAsset> FontCache =
            new Dictionary<string, TMP_FontAsset>();

        [SerializeField]
        private TMP_Text labelText;

        [SerializeField]
        private TMP_Text valueText;

        /// <summary>Raised after the player moves this row's setting to a new value.</summary>
        public event Action<SettingRowView> Changed;

        /// <summary>The setting this row is currently showing, or null before it is bound.</summary>
        public SettingDefinition Definition { get; private set; }

        /// <summary>
        /// Points this row at a setting and draws it. Safe to call more than once on the same
        /// row, though the menu currently builds a fresh row per rebuild.
        /// </summary>
        public void Bind(SettingDefinition definition)
        {
            this.Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (this.labelText != null)
            {
                this.labelText.text = definition.Label;
            }

            if (this.valueText != null)
            {
                var font = ResolveFont(definition.FontResourcePath);
                if (font != null)
                {
                    this.valueText.font = font;
                }
            }

            this.OnBind();
            this.Refresh();
        }

        /// <summary>Redraws the current value without changing it.</summary>
        public void Refresh()
        {
            if (this.valueText != null && this.Definition != null)
            {
                this.valueText.text = this.Definition.DisplayValue;
            }
        }

        /// <summary>
        /// Wires up whichever buttons the concrete row has. Called once per <see cref="Bind"/>,
        /// so implementations must clear their listeners before adding them.
        /// </summary>
        protected abstract void OnBind();

        /// <summary>
        /// Moves the bound setting by <paramref name="delta"/> options, redraws, and tells the
        /// menu about it.
        /// </summary>
        protected void Step(int delta)
        {
            if (this.Definition == null)
            {
                return;
            }

            this.Definition.Step(delta);
            this.Refresh();
            this.Changed?.Invoke(this);
        }

        private static TMP_FontAsset ResolveFont(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return null;
            }

            if (FontCache.TryGetValue(resourcePath, out var cached))
            {
                return cached;
            }

            TMP_FontAsset fontAsset = null;
            var font = Resources.Load<Font>(resourcePath);
            if (font != null)
            {
                fontAsset = TMP_FontAsset.CreateFontAsset(font);
            }

            FontCache[resourcePath] = fontAsset;
            return fontAsset;
        }
    }
}
