namespace Barbu.Editor
{
    using System.Collections.Generic;
    using Barbu.UI.Components;
    using Barbu.UI.Controllers;
    using TMPro;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// Builds the settings menu's prefabs and its scene hierarchy.
    /// </summary>
    /// <remarks>
    /// This exists so the menu's structure is written down rather than clicked together, and so
    /// it can be rebuilt after a change without redoing the wiring by hand. It is a build step,
    /// not a runtime one: once it has run, the prefabs and the scene objects are ordinary assets
    /// to edit in the inspector.
    ///
    /// Rebuild Menu Hierarchy replaces everything under SettingsMenu, so inspector tweaks made
    /// to those scene objects are lost on a rebuild. Tweaks to the row prefabs survive, because
    /// Rebuild Menu Hierarchy never overwrites an existing prefab.
    ///
    /// Sizes are in 1920x1080 reference pixels. The canvas scales with the screen, so they hold
    /// their proportions on any device rather than being fixed pixel sizes.
    /// </remarks>
    public static class SettingsMenuBuilder
    {
        private const string PrefabFolder = "Assets/Prefabs/UI";
        private const string SelectorRowPrefabPath = PrefabFolder + "/SelectorSettingRow.prefab";
        private const string ToggleRowPrefabPath = PrefabFolder + "/ToggleSettingRow.prefab";

        private const string SettingsMenuObjectName = "SettingsMenu";
        private const string SafeAreaObjectName = "SafeArea";

        private const int UILayer = 5;

        // The label takes 40% of a row and the controls 60%, as the stylesheet this replaced did.
        private const float LabelWidthShare = 2f;
        private const float ControlsWidthShare = 3f;

        private const float RowHeight = 64f;
        private const float ArrowButtonWidth = 64f;
        private const float ValueWidth = 200f;
        private const float ColumnSpacing = 16f;
        private const float ControlSpacing = 8f;
        private const float HeaderHeight = 72f;
        private const float FooterHeight = 88f;
        private const float SectionHeaderHeight = 60f;
        private const float CloseButtonWidth = 220f;
        private const float ResetButtonWidth = 260f;
        private const float FooterButtonHeight = 56f;
        private const float RowPaddingLeft = 16f;

        private const float HeaderFontSize = 36f;
        private const float SectionHeaderFontSize = 28f;
        private const float LabelFontSize = 28f;
        private const float ValueFontSize = 26f;
        private const float ValueFontSizeMin = 16f;

        private static readonly Color BackgroundColor = Color.white;

        // rgba(40, 161, 41, 1), the header green from the stylesheet this replaced.
        private static readonly Color HeaderColor = new Color(40f / 255f, 161f / 255f, 41f / 255f, 1f);
        private static readonly Color HeaderTextColor = Color.white;
        private static readonly Color ButtonColor = new Color(0.8784314f, 0.8784314f, 0.8784314f, 1f);

        // The arrows were transparent buttons before, so the glyph is the whole control. A
        // transparent Image still takes raycasts, so they stay tappable.
        private static readonly Color TransparentColor = new Color(1f, 1f, 1f, 0f);
        private static readonly Color TextColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

        [MenuItem("Barbu/Settings Menu/Create Row Prefabs", priority = 0)]
        public static void CreateRowPrefabs()
        {
            var existing = new List<string>();
            if (AssetDatabase.LoadAssetAtPath<GameObject>(SelectorRowPrefabPath) != null)
            {
                existing.Add(SelectorRowPrefabPath);
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(ToggleRowPrefabPath) != null)
            {
                existing.Add(ToggleRowPrefabPath);
            }

            if (existing.Count > 0 && !EditorUtility.DisplayDialog(
                    "Overwrite row prefabs?",
                    "This replaces the following prefabs with freshly generated ones, discarding any "
                    + "changes made to them in the inspector:\n\n" + string.Join("\n", existing),
                    "Overwrite",
                    "Cancel"))
            {
                return;
            }

            CreateRowPrefab(SelectorRowPrefabPath, BuildSelectorRow);
            CreateRowPrefab(ToggleRowPrefabPath, BuildToggleRow);
            AssetDatabase.SaveAssets();

            Debug.Log($"Settings row prefabs written to {PrefabFolder}.");
        }

        [MenuItem("Barbu/Settings Menu/Rebuild Menu Hierarchy", priority = 1)]
        public static void RebuildMenuHierarchy()
        {
            var settingsMenu = FindSettingsMenu();
            if (settingsMenu == null)
            {
                EditorUtility.DisplayDialog(
                    "Settings menu not found",
                    $"No root GameObject named '{SettingsMenuObjectName}' in the open scene. Open "
                    + "Assets/Scenes/Game.unity and try again.",
                    "OK");
                return;
            }

            var controller = settingsMenu.GetComponent<SettingsMenuController>();
            if (controller == null)
            {
                EditorUtility.DisplayDialog(
                    "Settings menu not found",
                    $"'{SettingsMenuObjectName}' has no {nameof(SettingsMenuController)} on it.",
                    "OK");
                return;
            }

            // Written but never overwritten here, so inspector tweaks to the rows survive a
            // hierarchy rebuild. Use Create Row Prefabs to deliberately regenerate them.
            var selectorPrefab = LoadOrCreateRowPrefab(SelectorRowPrefabPath, BuildSelectorRow);
            var togglePrefab = LoadOrCreateRowPrefab(ToggleRowPrefabPath, BuildToggleRow);

            ConfigureCanvas(settingsMenu);

            // Everything under SettingsMenu is generated, so clear all of it rather than a known
            // child name: the hierarchy's shape has changed before and will again.
            for (var i = settingsMenu.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(settingsMenu.transform.GetChild(i).gameObject);
            }

            BuildHierarchy(settingsMenu, controller, selectorPrefab, togglePrefab);

            EditorUtility.SetDirty(settingsMenu);
            EditorSceneManager.MarkSceneDirty(settingsMenu.scene);

            Debug.Log(
                "Settings menu hierarchy rebuilt. Save the scene to keep it. Tick the SettingsMenu "
                + "object active in the hierarchy to see the sample rows in the scene view.",
                settingsMenu);
        }

        private static GameObject FindSettingsMenu()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                return null;
            }

            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == SettingsMenuObjectName)
                {
                    return root;
                }
            }

            return null;
        }

        private static void ConfigureCanvas(GameObject settingsMenu)
        {
            // The menu used to be drawn by UI Toolkit; its document goes away with the UXML.
            var document = settingsMenu.GetComponent<UnityEngine.UIElements.UIDocument>();
            if (document != null)
            {
                Object.DestroyImmediate(document);
            }

            var rectTransform = settingsMenu.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // An overlay canvas drives its own scale at runtime, but a zero scale left over
                // from the UI Toolkit setup would hide the menu in the scene view.
                rectTransform.localScale = Vector3.one;
            }

            var canvas = EnsureComponent<Canvas>(settingsMenu);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Above the main and single round menus, which sit at 2.
            canvas.sortingOrder = 4;

            var scaler = EnsureComponent<CanvasScaler>(settingsMenu);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            // The game is landscape, so height is the dimension worth holding constant.
            scaler.matchWidthOrHeight = 1f;

            EnsureComponent<GraphicRaycaster>(settingsMenu);
        }

        private static void BuildHierarchy(
            GameObject settingsMenu,
            SettingsMenuController controller,
            GameObject selectorPrefab,
            GameObject togglePrefab)
        {
            // The panel fills the screen: this is a full screen menu, not a dialog floating over
            // the game.
            var panel = CreateUIObject("SettingsPanel", settingsMenu.transform);
            Stretch(panel.GetComponent<RectTransform>());

            // A solid fill rather than the sliced UI sprite: at full screen that sprite's
            // rounded corners would let the game show through at the corners of the screen.
            AddSolidBackground(panel, BackgroundColor);

            var panelLayout = panel.AddComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(0, 0, 0, 0);
            panelLayout.spacing = 0f;
            panelLayout.childAlignment = TextAnchor.UpperCenter;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            // The green band runs the full width, outside the safe area, so a notch cuts into
            // the header rather than leaving a white gap beside it.
            var header = CreateUIObject("Header", panel.transform);
            AddSolidBackground(header, HeaderColor);
            AddLayoutElement(header, preferredHeight: HeaderHeight);

            var headerLabel = CreateUIObject("HeaderLabel", header.transform);
            Stretch(headerLabel.GetComponent<RectTransform>());
            var headerText = AddText(headerLabel, "Settings", HeaderFontSize, TextAlignmentOptions.Center, FontStyles.Bold);
            headerText.color = HeaderTextColor;

            // Only the rows are held clear of a notch, which is what the left and right spacers
            // in the old layout did. Its padding is what ApplySafeArea drives.
            var body = CreateUIObject(SafeAreaObjectName, panel.transform);
            AddLayoutElement(body, flexibleHeight: 1f);
            var bodyLayout = body.AddComponent<HorizontalLayoutGroup>();
            bodyLayout.childControlWidth = true;
            bodyLayout.childControlHeight = true;
            bodyLayout.childForceExpandWidth = true;
            bodyLayout.childForceExpandHeight = true;

            var scrollView = CreateUIObject("ScrollView", body.transform);

            // Not decoration: the ScrollRect needs a graphic here to receive drags.
            AddSolidBackground(scrollView, BackgroundColor);
            var scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
            scrollRect.scrollSensitivity = 30f;

            var viewport = CreateUIObject("Viewport", scrollView.transform);
            var viewportRect = viewport.GetComponent<RectTransform>();
            Stretch(viewportRect);
            viewport.AddComponent<RectMask2D>();

            var content = CreateUIObject("Content", viewport.transform);
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(16, 16, 16, 16);
            contentLayout.spacing = 12f;
            contentLayout.childAlignment = TextAnchor.UpperCenter;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = true;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            // The row list has no fixed length, so the content grows to fit and the viewport
            // scrolls it rather than the rows being squeezed.
            var fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

            // Samples so the menu can be judged in the scene view. The controller switches them
            // off before it builds the real rows.
            var previewSelector = InstantiatePreview(selectorPrefab, content.transform, "PreviewSelectorRow");
            var previewToggle = InstantiatePreview(togglePrefab, content.transform, "PreviewToggleRow");

            var featureFlagHeader = CreateUIObject("FeatureFlagHeader", content.transform);
            AddText(featureFlagHeader, "Feature Flags", SectionHeaderFontSize, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            AddLayoutElement(featureFlagHeader, preferredHeight: SectionHeaderHeight);
            featureFlagHeader.SetActive(false);

            var noFeatureFlags = CreateUIObject("NoFeatureFlagsLabel", content.transform);
            AddText(noFeatureFlags, "No feature flags defined.", ValueFontSize, TextAlignmentOptions.MidlineLeft);
            AddLayoutElement(noFeatureFlags, preferredHeight: RowHeight);
            noFeatureFlags.SetActive(false);

            var footer = CreateUIObject("Footer", panel.transform);
            AddLayoutElement(footer, preferredHeight: FooterHeight);
            var footerLayout = footer.AddComponent<HorizontalLayoutGroup>();
            footerLayout.spacing = 16f;
            footerLayout.childAlignment = TextAnchor.MiddleCenter;
            footerLayout.childControlWidth = true;
            footerLayout.childControlHeight = true;

            // Not expanded: the buttons keep the size their LayoutElement asks for and sit
            // centred, rather than stretching across the footer.
            footerLayout.childForceExpandWidth = false;
            footerLayout.childForceExpandHeight = false;

            var resetOverrides = CreateUIObject("ResetOverridesButton", footer.transform);
            var resetOverridesButton = AddButton(resetOverrides);
            AddLayoutElement(resetOverrides, preferredWidth: ResetButtonWidth, preferredHeight: FooterButtonHeight);
            AddCenteredChildText(resetOverrides, "Reset Overrides", ValueFontSize);
            resetOverrides.SetActive(false);

            var close = CreateUIObject("CloseButton", footer.transform);
            var closeButton = AddButton(close);
            AddLayoutElement(close, preferredWidth: CloseButtonWidth, preferredHeight: FooterButtonHeight);
            AddCenteredChildText(close, "Close", LabelFontSize, FontStyles.Bold);

            SetField(controller, "safeAreaPadding", bodyLayout);
            SetField(controller, "scrollRect", scrollRect);
            SetField(controller, "contentRoot", contentRect);
            SetField(controller, "closeButton", closeButton);
            SetField(controller, "selectorRowPrefab", selectorPrefab.GetComponent<SelectorSettingRowView>());
            SetField(controller, "toggleRowPrefab", togglePrefab.GetComponent<ToggleSettingRowView>());
            SetField(controller, "featureFlagHeader", featureFlagHeader);
            SetField(controller, "noFeatureFlagsLabel", noFeatureFlags);
            SetField(controller, "resetOverridesButton", resetOverridesButton);
            SetArrayField(controller, "previewRows", new Object[] { previewSelector, previewToggle });
        }

        private static GameObject BuildSelectorRow()
        {
            var root = CreateRowRoot("SelectorSettingRow", out var labelText, out var controls);

            var previous = CreateUIObject("PreviousButton", controls.transform);
            var previousButton = AddButton(previous, TransparentColor);
            AddLayoutElement(previous, preferredWidth: ArrowButtonWidth);
            AddCenteredChildText(previous, "<", LabelFontSize, FontStyles.Bold);

            var valueText = CreateValueBox(controls.transform);

            var next = CreateUIObject("NextButton", controls.transform);
            var nextButton = AddButton(next, TransparentColor);
            AddLayoutElement(next, preferredWidth: ArrowButtonWidth);
            AddCenteredChildText(next, ">", LabelFontSize, FontStyles.Bold);

            var view = root.AddComponent<SelectorSettingRowView>();
            SetField(view, "labelText", labelText);
            SetField(view, "valueText", valueText);
            SetField(view, "previousButton", previousButton);
            SetField(view, "nextButton", nextButton);

            return root;
        }

        private static GameObject BuildToggleRow()
        {
            var root = CreateRowRoot("ToggleSettingRow", out var labelText, out var controls);

            var toggle = CreateUIObject("ToggleButton", controls.transform);
            var toggleButton = AddButton(toggle);
            AddLayoutElement(toggle, preferredWidth: ValueWidth, preferredHeight: FooterButtonHeight);
            var valueText = AddCenteredChildText(toggle, "Off", ValueFontSize);

            var view = root.AddComponent<ToggleSettingRowView>();
            SetField(view, "labelText", labelText);
            SetField(view, "valueText", valueText);
            SetField(view, "toggleButton", toggleButton);

            return root;
        }

        /// <summary>
        /// The half of a row that both prefabs share: a label on the left that takes the slack,
        /// and a fixed width column of controls on the right so the controls line up down the
        /// menu however long the labels are.
        /// </summary>
        private static GameObject CreateRowRoot(string name, out TMP_Text labelText, out GameObject controls)
        {
            var root = CreateUIObject(name, null);
            var rowLayout = root.AddComponent<HorizontalLayoutGroup>();
            rowLayout.padding = new RectOffset((int)RowPaddingLeft, 0, 0, 0);
            rowLayout.spacing = ColumnSpacing;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = true;
            AddLayoutElement(root, preferredHeight: RowHeight, flexibleWidth: 1f);

            // A preferred width of zero on both columns hands the whole row to the flexible
            // shares below, so the controls line up down the menu however long a label runs.
            var label = CreateUIObject("Label", root.transform);
            labelText = AddText(label, "Setting", LabelFontSize, TextAlignmentOptions.MidlineLeft);
            AddLayoutElement(label, preferredWidth: 0f, flexibleWidth: LabelWidthShare);

            controls = CreateUIObject("Controls", root.transform);
            var controlsLayout = controls.AddComponent<HorizontalLayoutGroup>();
            controlsLayout.spacing = ControlSpacing;
            controlsLayout.childAlignment = TextAnchor.MiddleLeft;
            controlsLayout.childControlWidth = true;
            controlsLayout.childControlHeight = true;
            controlsLayout.childForceExpandWidth = false;

            // Height comes from the row; without this the controls have no content of their own
            // to size against and collapse.
            controlsLayout.childForceExpandHeight = true;
            AddLayoutElement(controls, preferredWidth: 0f, flexibleWidth: ControlsWidthShare);

            return root;
        }

        /// <summary>
        /// The current value, as plain centred text on the background rather than in a box,
        /// which is how the stylesheet this replaced drew it.
        /// </summary>
        private static TMP_Text CreateValueBox(Transform parent)
        {
            var value = CreateUIObject("Value", parent);
            var valueText = AddText(value, "Value", ValueFontSize, TextAlignmentOptions.Center);
            AddLayoutElement(value, preferredWidth: ValueWidth);

            // Option text varies a lot in length, so let the longest ones shrink to fit rather
            // than overflow their column.
            valueText.enableAutoSizing = true;
            valueText.fontSizeMin = ValueFontSizeMin;
            valueText.fontSizeMax = ValueFontSize;

            return valueText;
        }

        private static GameObject LoadOrCreateRowPrefab(string path, System.Func<GameObject> build)
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return existing != null ? existing : CreateRowPrefab(path, build);
        }

        private static GameObject CreateRowPrefab(string path, System.Func<GameObject> build)
        {
            EnsureFolder("Assets", "Prefabs");
            EnsureFolder("Assets/Prefabs", "UI");

            var temporary = build();
            var prefab = PrefabUtility.SaveAsPrefabAsset(temporary, path);
            Object.DestroyImmediate(temporary);
            return prefab;
        }

        private static GameObject InstantiatePreview(GameObject prefab, Transform parent, string name)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = name;
            instance.SetActive(true);
            return instance;
        }

        private static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform)) { layer = UILayer };
            gameObject.GetComponent<RectTransform>().SetParent(parent, worldPositionStays: false);
            return gameObject;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static T EnsureComponent<T>(GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        /// <summary>
        /// A plain filled rectangle, with no sprite and so no rounded corners.
        /// </summary>
        private static Image AddSolidBackground(GameObject gameObject, Color color)
        {
            var image = gameObject.AddComponent<Image>();
            image.sprite = null;
            image.type = Image.Type.Simple;
            image.color = color;
            return image;
        }

        private static Image AddBackground(GameObject gameObject, Color color)
        {
            var image = gameObject.AddComponent<Image>();
            image.sprite = UISprite();
            image.type = Image.Type.Sliced;
            image.color = color;
            return image;
        }

        private static Button AddButton(GameObject gameObject, Color? color = null)
        {
            var image = AddBackground(gameObject, color ?? ButtonColor);
            var button = gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }

        private static TMP_Text AddText(
            GameObject gameObject,
            string text,
            float fontSize,
            TextAlignmentOptions alignment,
            FontStyles fontStyle = FontStyles.Normal)
        {
            var label = gameObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = alignment;
            label.fontStyle = fontStyle;
            label.color = TextColor;

            // Text never needs to swallow a click; the button underneath does.
            label.raycastTarget = false;
            return label;
        }

        private static TMP_Text AddCenteredChildText(
            GameObject parent,
            string text,
            float fontSize,
            FontStyles fontStyle = FontStyles.Normal)
        {
            var child = CreateUIObject("Text", parent.transform);
            Stretch(child.GetComponent<RectTransform>());
            return AddText(child, text, fontSize, TextAlignmentOptions.Center, fontStyle);
        }

        private static LayoutElement AddLayoutElement(
            GameObject gameObject,
            float preferredWidth = -1f,
            float preferredHeight = -1f,
            float flexibleWidth = -1f,
            float flexibleHeight = -1f)
        {
            var element = gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = preferredWidth;
            element.preferredHeight = preferredHeight;
            element.flexibleWidth = flexibleWidth;
            element.flexibleHeight = flexibleHeight;
            return element;
        }

        private static Sprite UISprite()
        {
            return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }

        private static void SetField(Object target, string fieldName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogError($"{target.GetType().Name} has no serialized field named '{fieldName}'.");
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetArrayField(Object target, string fieldName, Object[] values)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                Debug.LogError($"{target.GetType().Name} has no serialized field named '{fieldName}'.");
                return;
            }

            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
