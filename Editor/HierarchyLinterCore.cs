namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class HierarchyLinterCore
    {
        #region Data

        private static readonly List<IUnityUILinter> Linters = new()
        {
            new UnityUIHierarchyNamingLinter(),
            new RectTransformValuesLinter(),
            new LayoutGroupExclusivityLinter(),
            // TODO: Ensure logic separation
        };

        #endregion

        #region Lifecycle

        static HierarchyLinterCore()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            Debug.Log("Linter loaded.");
        }

        private static void OnHierarchyChanged()
        {
            // Debug.Log("hierarchy changed in Edit Mode");
            Lint();
        }

        #endregion

        #region Methods

        private static Canvas[] FindUIHierarchyRoots()
        {
            // Canvas is the root component of every Unity UI hierarchy.
            var canvases = Object.FindObjectsOfType<Canvas>();

            return canvases;
        }

        private static void Lint()
        {
            if (Application.isPlaying)
            {
                // Only lint in Edit mode
                return;
            }

            // var hierarchyRoots = FindUIHierarchyRoots();
            var rects = Object.FindObjectsOfType<RectTransform>(true);

            foreach (var linter in Linters)
            foreach (var rect in rects)
            {
                // Apply each linter to all UI rects
                linter.Lint(rect);
            }
        }

        #endregion
    }
}