using System;
using System.Collections.Generic;
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

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    /// <summary>
    /// Applies pre-defined naming rules to GameObject belonging to the linted RectTransform.
    /// </summary>
    internal class RectTransformNamingLinter : IUnityUILinter
    {
        #region Data
        
        private const int MaxContentLabelChars = 20;
        private const string LabelSeparator = " ⁃ ";
        private const char TextQuoteOpeningChar = '“';
        private const char TextQuoteClosingChar = '”';
        private const string EmptyTextString = "<no text>";
        private const string EmptyImageString = "<no texture>";

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

            { typeof(HorizontalLayoutGroup), "Layout ↔" },
            { typeof(VerticalLayoutGroup), "Layout ↕" },
            { typeof(GridLayoutGroup), "Layout ⋮⋮⋮" },

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
        
        #endregion

        #region Linter callbacks

        public void Lint(RectTransform rect)
        {
            EnforceNaming(rect);
        }

        #endregion

        #region Logic

        private static void EnforceNaming(RectTransform rect)
        {
            var gameObject = rect.gameObject;

            string typeString = GetTypeStringForRect(rect);
            string name = typeString;

            string contentString = GetContentStringForRect(rect);
            if (!string.IsNullOrEmpty(contentString)) name += $"{LabelSeparator}{contentString}";

            string customLabelString = GetCustomLabelStringForRect(rect);
            if (!string.IsNullOrEmpty(customLabelString)) name += $"{LabelSeparator}{customLabelString}";
            
            gameObject.name = name;
        }

        /// <summary>
        /// Retrieves the UI component with the highest naming priority attached to the given <see cref="RectTransform"/>.
        /// </summary>
        /// <remarks>
        /// In order to give meaningful names to the GameObjects in the UI hierarchy, we have to look for the UI components 
        /// attached to them. However, often GameObjects will have multiple UI components on them. In this case, we have 
        /// to pick one that has the highest priority in the given context. We do this by following the order of priority
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
        /// <remarks>
        /// Type labels are strings which allow to discern a UI element based on its type.
        /// </remarks>
        /// <param name="rect"><see cref="RectTransform"/> to get the label for.</param>
        /// <returns>Label string.</returns>
        private static string GetTypeStringForRect(RectTransform rect)
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
        /// Content labels are strings which allow to discern a UI element based on its content.
        /// For example, for Buttons, the title is the content of Text element belonging to the Button (if any).
        /// </remarks>
        /// <param name="rect"><see cref="RectTransform"/> to get the title for.</param>
        /// <returns>Title string.</returns>
        private static string GetContentStringForRect(RectTransform rect)
        {
            var gameObject = rect.gameObject;
            var uiComponent = GetHighestNamingPriorityUIComponent(rect);

            // Text-based components
            if (IsTextComponent(uiComponent))
            {
                string text = GetTextValueFromChildren(rect);
                if (string.IsNullOrEmpty(text)) return EmptyTextString;

                text = text.Split("\n")[0];
                return QuoteText(text.EllipsizeMultiline(MaxContentLabelChars));
            }

            // Image-based
            if (IsImageComponent(uiComponent))
            {
                return uiComponent switch
                {
                    // Images are named after their attached texture or sprite
                    Image i => (i.sprite != null) ? i.sprite.name : EmptyImageString,
                    RawImage i => (i.texture != null) ? i.texture.name : EmptyImageString,
                    _ => null,
                };
            }

            return null;
        }

        private static string GetCustomLabelStringForRect(RectTransform rect)
        {
            var customLabel = rect.GetComponent<UILinterObjectNameLabel>();
            if (customLabel == null) return null;
            
            return customLabel.nameLabel;
        }

        #region Images

        /// <summary>
        /// Set of UI component types which labels should be formatted based on attached images.
        /// </summary>
        private static HashSet<Type> ImageUIComponents = new()
        {
            typeof(Image),
            typeof(RawImage),
        };

        private static bool IsImageComponent(Component uiComponent)
        {
            return ImageUIComponents.Contains(uiComponent.GetType());
        }

        #endregion

        #region Text

        /// <summary>
        /// Set of UI component types which labels should be formatted based on attached text.
        /// </summary>
        private static HashSet<Type> TextUIComponents = new()
        {
            typeof(Text),
            typeof(TextMeshProUGUI),
            typeof(Toggle),
            typeof(ToggleGroup),
            typeof(InputField),
            typeof(TMP_InputField),
            typeof(Button),
        };

        private static bool IsTextComponent(Component uiComponent)
        {
            return TextUIComponents.Contains(uiComponent.GetType());
        }

        /// <summary>
        /// Surrounds the given string in quotes.
        /// </summary>
        private static string QuoteText(string text)
        {
            return $"{TextQuoteOpeningChar}{text}{TextQuoteClosingChar}";
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

        #endregion

        #endregion
    }
}