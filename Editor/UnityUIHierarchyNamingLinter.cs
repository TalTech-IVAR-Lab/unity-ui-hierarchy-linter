namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private const int MaxTitleChars = 10;

        private static readonly Dictionary<Type, string> UIComponentLabels = new()
        {
            { typeof(RectTransform), "Group" },
            
            { typeof(Canvas), "UI Canvas" },
            { typeof(CanvasGroup), "UI Canvas Group" },
            
            { typeof(Mask), "Mask" },
            { typeof(RectMask2D), "Rect Mask" },
            
            { typeof(Image), "Image" },
            { typeof(RawImage), "Raw Image" },

            { typeof(Text), "Text" },
            { typeof(TextMeshProUGUI), "Text" },

            { typeof(Toggle), "Toggle" },
            { typeof(Slider), "Slider" },
            { typeof(Scrollbar), "Scrollbar" },
            { typeof(ScrollView), "Scroll View" },
            { typeof(Button), "Button" },
            { typeof(Dropdown), "Dropdown" },
            { typeof(TMP_Dropdown), "Dropdown" },
            { typeof(InputField), "Input Field" },
            { typeof(TMP_InputField), "Input Field" },

            { typeof(HorizontalLayoutGroup), "Layout / ↔ Horizontal" },
            { typeof(VerticalLayoutGroup), "Layout / ↕ Vertical" },
            { typeof(GridLayoutGroup), "Layout / ▦ Grid" },
        };

        private static List<Type> UIComponentsTypeHierarchy = new()
        {
            typeof(Canvas),
            typeof(CanvasGroup),
            
            typeof(HorizontalLayoutGroup),
            typeof(VerticalLayoutGroup),
            typeof(GridLayoutGroup),
            
            typeof(Mask),
            typeof(RectMask2D),
            
            typeof(TextMeshProUGUI),
            typeof(Text),
            
            typeof(RectTransform),
        };

        private const string LabelTitleSeparator = " - ";

        private static void EnforceNaming(RectTransform rect)
        {
            var gameObject = rect.gameObject;

            string label = GetLabelForRect(rect);
            string title = GetTitleForRect(rect);

            string name = $"{label}{LabelTitleSeparator}{title}";

            gameObject.name = name;
        }

        private static Component GetMainUIComponent(RectTransform rect)
        {
            var canvas = rect.GetComponent<Canvas>();
            if (canvas != null)
            {
                return canvas;
            }

            var uiBehaviours = rect.GetComponents<UIBehaviour>();
            if (uiBehaviours.Length > 0)
            {
                // Sort by priority and select the top one
                // TODO: define priority
                // TODO: sort
                return uiBehaviours[0];
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
            var uiComponent = GetMainUIComponent(rect);
            if (uiComponent == null)
            {
                // Rect doesn't have any UI components attached to it - it's a simple container
                return "Container";
            }
            
            var type = uiComponent.GetType();

            UIComponentLabels.TryGetValue(type, out string name);
            return $"{name}";
        }

        /// <summary>
        /// Gets a title string for the given UI component.
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
                var mainUIComponent = GetMainUIComponent(rect);
                if (mainUIComponent != null)
                {
                    title = GetAssociatedTextValue(mainUIComponent)?.Ellipsize(MaxTitleChars);
                }
            }
            
            return title;
        }

        /// <summary>
        /// Tries to retrieve the text associated with the given UI element.
        /// </summary>
        /// <remarks>
        /// This text's value is used as a default title for UI GameObjects.
        /// </remarks>
        /// <param name="uiComponent">UI <see cref="Component"/> to look for the text value for.</param>
        /// <returns>Text string or null, if not applicable to the given <see cref="UIBehaviour"/> type.</returns>
        private static string GetAssociatedTextValue(Component uiComponent)
        {
            string text = uiComponent switch
            {
                // Text speaks for itself
                Text t => t.text,
                TMP_Text t => t.text,
                
                // Images are be named after their attached texture or sprite
                Image i => (i.sprite != null) ? i.sprite.name : null,
                RawImage i => (i.texture != null) ? i.texture.name : null,

                // Layout groups don't have any default text associated with them
                HorizontalLayoutGroup => null,
                VerticalLayoutGroup => null,
                GridLayoutGroup => null,
                
                // Canvases don't have any default text associated with them
                Canvas => null,
                
                // Other component types have associated text in child objects
                _ => GetTextValueFromChildren(uiComponent)
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
            var text = uiComponent.GetComponentInChildren<TMP_Text>();
            if (text != null) return text.text;

            var legacyText = uiComponent.GetComponentInChildren<Text>();
            if (legacyText != null) return legacyText.text;

            return null;
        }
        
        public void Lint(RectTransform rect)
        {
            EnforceNaming(rect);
        }
    }
}