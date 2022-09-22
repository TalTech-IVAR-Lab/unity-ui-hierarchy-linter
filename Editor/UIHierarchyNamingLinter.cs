namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using Button = UnityEngine.UI.Button;
    using Image = UnityEngine.UI.Image;
    using Slider = UnityEngine.UI.Slider;
    using Toggle = UnityEngine.UI.Toggle;

    internal class UIHierarchyNamingLinter : ICanvasLinter
    {
        private const int MaxLabelChars = 10;

        private static readonly Dictionary<Type, string> UIComponentLabels = new()
        {
            { typeof(Image), "Image" },
            { typeof(RawImage), "Raw Image" },

            { typeof(Text), "Text" },
            { typeof(TMP_Text), "Text" },

            { typeof(Toggle), "Toggle" },
            { typeof(Slider), "Slider" },
            { typeof(Scrollbar), "Scrollbar" },
            { typeof(ScrollView), "Scroll View" },
            { typeof(Button), "Button" },
            { typeof(Dropdown), "Dropdown" },
            { typeof(TMP_Dropdown), "Dropdown" },
            { typeof(InputField), "Input Field" },

            { typeof(HorizontalLayoutGroup), "Layout / ↔ Horizontal" },
            { typeof(VerticalLayoutGroup), "Layout / ↕ Vertical" },
            { typeof(GridLayoutGroup), "Layout / ▦ Grid" },
        };

        private const string LabelTitleSeparator = " - ";

        private static void LintHierarchyNaming(Canvas hierarchyRootCanvas)
        {
            var uiComponents = hierarchyRootCanvas.GetComponentsInChildren<UIBehaviour>();

            foreach (var uiComponent in uiComponents) { EnforceGameObjectNaming(uiComponent); }
        }

        private static void EnforceGameObjectNaming(UIBehaviour uiComponent)
        {
            var gameObject = uiComponent.gameObject;

            string label = GetLabelForUIGameObject(uiComponent);
            string title = GetTitleForUIGameObject(uiComponent);

            string name = $"{label}{LabelTitleSeparator}{title}";

            gameObject.name = name;
        }

        /// <summary>
        /// Gets a label string for the given UI GameObject.
        /// </summary>
        /// <param name="uiComponent">UI component to get the label for.</param>
        /// <returns>Label string.</returns>
        private static string GetLabelForUIGameObject(UIBehaviour uiComponent)
        {
            var type = uiComponent.GetType();

            string label = UIComponentLabels.TryGetValue(type, out string name) ? name : "";
            return $"[{label}]";
        }

        /// <summary>
        /// Gets a title string for the given UI component.
        /// </summary>
        /// <remarks>
        /// Titles are user-defined strings which allow to discern a UI element based on it's content.
        /// For example, for Buttons, the title is the content of Text element belonging to the Button (if any).
        /// </remarks>
        /// <param name="uiComponent">UI component to get the title for.</param>
        /// <returns>Title string.</returns>
        private static string GetTitleForUIGameObject(UIBehaviour uiComponent)
        {
            var gameObject = uiComponent.gameObject;

            // Remember original title in the GameObject name
            string label = GetLabelForUIGameObject(uiComponent);
            string prefix = $"{label}{LabelTitleSeparator}";
            string originalTitle = gameObject.name.RemovePrefix(prefix);
            if (originalTitle == "") originalTitle = null;

            // Set title based on the prevalent UI component type on this GameObject
            string title = uiComponent switch
            {
                Text t => originalTitle ?? t.text.Ellipsize(MaxLabelChars),
                TMP_Text t => originalTitle ?? t.text.Ellipsize(MaxLabelChars),
                Toggle t => t.GetComponentInChildren<Text>()?.text ?? originalTitle,
                _ => originalTitle
            };

            return title;
        }

        /// <summary>
        /// Tries to retrieve the text associated with the given UI element.
        /// </summary>
        /// <remarks>
        /// This text's value is used as a default title for UI GameObjects.
        /// </remarks>
        /// <param name="uiBehaviour"></param>
        /// <returns></returns>
        private string GetAssociatedTextValue(UIBehaviour uiBehaviour)
        {
            string text = uiBehaviour switch
            {
                // Text speaks for itself
                Text t => t.text,
                TMP_Text t => t.text,

                // Layout groups don't have any default text associated with them
                HorizontalLayoutGroup => null,
                VerticalLayoutGroup => null,
                GridLayoutGroup => null,
                
                // Other component types have associated text in child objects
                _ => GetTextValueFromChildren(uiBehaviour)
            };

            return text;
        }
        
        /// <summary>
        /// Returns string value of the first <see cref="TMP_Text"/> or <see cref="Text"/> from children of the given <see cref="UIBehaviour"/>, if available.
        /// </summary>
        /// <param name="uiBehaviour"><see cref="UIBehaviour"/> to search for the text value in.</param>
        /// <returns>Text string if found, null otherwise.</returns>
        private static string GetTextValueFromChildren(UIBehaviour uiBehaviour)
        {
            var text = uiBehaviour.GetComponentInChildren<TMP_Text>();
            if (text != null) return text.text;

            var legacyText = uiBehaviour.GetComponentInChildren<Text>();
            if (legacyText != null) return legacyText.text;

            return null;
        }

        private static string GetTitleFromUIGameObject(GameObject gameObject)
        {
            // string[] split = gameObject.name.Split(new[] { LabelTitleSeparator }, StringSplitOptions.None);
            // if (split.Length < 2)
            // {
            //     // Name contained no separator, special case
            //     return split[0];
            // }
            //
            // Title
            // // sting[] splitWithoutTag
            //
            // string label = "";

            return "";
        }

        public void Lint(Canvas canvas) { throw new NotImplementedException(); }
    }
}