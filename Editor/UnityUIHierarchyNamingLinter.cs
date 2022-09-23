namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using System;
    using System.Collections.Specialized;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using Button = UnityEngine.UI.Button;
    using Image = UnityEngine.UI.Image;
    using Slider = UnityEngine.UI.Slider;
    using Toggle = UnityEngine.UI.Toggle;

    /// <summary>
    /// Applies pre-defined naming rules to GameObject belonging to the linted RectTransform.
    /// </summary>
    internal class UnityUIHierarchyNamingLinter : IUnityUILinter
    {
        private const int MaxTitleChars = 20;
        private const string EmptyTextString = "<empty>";

        /// <summary>
        /// Ordered dictionary that matches Unity UI component types with their respective pretty names.
        /// </summary>
        /// <remarks>
        /// The order of this dict serves a purpose: it defines the naming priority of UI components when
        /// selecting the main component on the <see cref="RectTransform"/> for producing default labels and
        /// titles for UI GameObjects.
        /// </remarks>
        /// <seealso cref="GetHighestNamingPriorityUIComponent"/>
        private static readonly OrderedDictionary UIComponentsNames = new()
        {
            { typeof(Canvas), "UI Canvas" },
            { typeof(CanvasGroup), "UI Canvas Group" },

            { typeof(HorizontalLayoutGroup), "Layout / ↔ Horizontal" },
            { typeof(VerticalLayoutGroup), "Layout / ↕ Vertical" },
            { typeof(GridLayoutGroup), "Layout / ▦ Grid" },

            { typeof(ScrollView), "Scroll View" },
            { typeof(ScrollRect), "Scroll Rect" },
            { typeof(Scrollbar), "Scrollbar" },
            { typeof(Slider), "Slider" },
            { typeof(Toggle), "Toggle" },
            { typeof(ToggleGroup), "Toggle Group" },
            { typeof(Dropdown), "Dropdown" },
            { typeof(TMP_Dropdown), "Dropdown" },
            { typeof(InputField), "Input Field" },
            { typeof(TMP_InputField), "Input Field" },
            { typeof(Button), "Button" },

            { typeof(Mask), "Mask" },
            { typeof(RectMask2D), "Rect Mask" },

            { typeof(Text), "Text" },
            { typeof(TextMeshProUGUI), "Text" },

            { typeof(Image), "Image" },
            { typeof(RawImage), "Raw Image" },

            { typeof(RectTransform), "Group" },
        };

        private const string LabelTitleSeparator = " ⁃ ";

        private static void EnforceNaming(RectTransform rect)
        {
            var gameObject = rect.gameObject;

            string label = GetLabelForRect(rect);
            string title = GetTitleForRect(rect);

            string name = $"{label}";

            if (!string.IsNullOrEmpty(title)) name += $"{LabelTitleSeparator}{title}";

            gameObject.name = name;
        }

        /// <summary>
        /// Retrieves the UI component with the highest naming priority attached to the given <see cref="RectTransform"/>.
        /// </summary>
        /// <remarks>
        /// In order to give meaningful names to the GameObjects in the UI hierarchy, we have to look for the UI components 
        /// attached to them. However, often GameObjects will have multiple UI components attached to them. In this case, we
        /// have to pick one that has the highest priority in the given context. We do this by following the order of priority
        /// defined in <see cref="UIComponentsNames"/>.
        /// </remarks>
        /// <param name="rect"><see cref="RectTransform"/> to look for the UI component on.</param>
        private static Component GetHighestNamingPriorityUIComponent(RectTransform rect)
        {
            var allComponents = rect.GetComponents<Component>();
            
            // Find the first matching UI component on the rect following the pre-defined order
            foreach (Type componentType in UIComponentsNames.Keys)
            foreach (var component in allComponents)
            {
                // The first match will be the highest-priority component on this rect
                if (componentType == component.GetType()) return component;
            }

            return rect;
        }

        /// <summary>
        /// Gets a label string for the given UI GameObject.
        /// </summary>
        /// <param name="rect"><see cref="RectTransform"/> to get the label for.</param>
        /// <returns>Label string.</returns>
        private static string GetLabelForRect(RectTransform rect)
        {
            var uiComponent = GetHighestNamingPriorityUIComponent(rect);
            var type = uiComponent.GetType();

            string componentName = null;
            if (UIComponentsNames.Contains(type))
            {
                componentName = (string)UIComponentsNames[type];
            }

            return $"{componentName}";
        }

        /// <summary>
        /// Gets a title string for the given <see cref="RectTransform"/>.
        /// </summary>
        /// <remarks>
        /// Titles are user-defined strings which allow to discern a UI element based on it's content.
        /// For example, for Buttons, the title is the content of Text element belonging to the Button (if any).
        /// </remarks>
        /// <param name="rect"><see cref="RectTransform"/> to get the title for.</param>
        /// <returns>Title string.</returns>
        private static string GetTitleForRect(RectTransform rect)
        {
            var gameObject = rect.gameObject;

            // Remember original title in the GameObject name
            string label = GetLabelForRect(rect);
            string prefix = $"{label}{LabelTitleSeparator}";
            string title = gameObject.name.RemovePrefix(prefix);

            // If title is empty, set it based on the prevalent UI component type on this GameObject
            bool strictTitle = true;
            if (title == "" || strictTitle)
            {
                var mainUIComponent = GetHighestNamingPriorityUIComponent(rect);
                if (mainUIComponent != null) { title = GetDefaultTitle(mainUIComponent)?.Ellipsize(MaxTitleChars); }
            }

            return title;
        }

        /// <summary>
        /// Tries to retrieve a default title for the given UI element.
        /// </summary>
        /// <remarks>
        /// For example, a default title for a Button will be this Button's nested Text content.
        /// </remarks>
        /// <param name="uiComponent">UI <see cref="Component"/> to look for the text value for.</param>
        /// <returns>Text string or null, if not applicable to the given <see cref="UIBehaviour"/> type.</returns>
        private static string GetDefaultTitle(Component uiComponent)
        {
            // Helper function to extract text string value from both TMP and legacy Text components
            string ExtractText(dynamic t) => string.IsNullOrEmpty(t.text) ? EmptyTextString : t.text;
            
            string text = uiComponent switch
            {
                // Text speaks for itself
                Text t => ExtractText(t),
                TextMeshProUGUI t => ExtractText(t),

                // Images are named after their attached texture or sprite
                Image i => (i.sprite != null) ? i.sprite.name : null,
                RawImage i => (i.texture != null) ? i.texture.name : null,
                
                // Component types which can have associated text in their child objects
                Toggle => GetTextValueFromChildren(uiComponent),
                ToggleGroup => GetTextValueFromChildren(uiComponent),
                InputField => GetTextValueFromChildren(uiComponent),
                TMP_InputField => GetTextValueFromChildren(uiComponent),
                Button => GetTextValueFromChildren(uiComponent),

                // Other component types don't have any default text associated with them
                _ => null,
            };

            return text;
        }

        /// <summary>
        /// Returns string value of the first <see cref="TMP_Text"/> or <see cref="Text"/> from children of the given <see cref="UIBehaviour"/>, if available.
        /// </summary>
        /// <param name="uiComponent">Root <see cref="Component"/> to look up the text value from.</param>
        /// <returns>Text string if found, null otherwise.</returns>
        private static string GetTextValueFromChildren(Component uiComponent)
        {
            var text = uiComponent.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) return text.text;

            var legacyText = uiComponent.GetComponentInChildren<Text>();
            if (legacyText != null) return legacyText.text;

            return null;
        }

        public void Lint(RectTransform rect) { EnforceNaming(rect); }
    }
}