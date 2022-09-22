namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class HierarchyLinterCore
    {
        #region Data

        private static readonly List<ICanvasLinter> Linters = new()
        {
            new UIHierarchyNamingLinter()
            // TODO: Ensure logic separation
            // TODO: Clean RectTransforms
            // TODO: Enforce exclusivity of LayoutGroups
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
            
            var hierarchyRoots = FindUIHierarchyRoots();

            foreach (var linter in Linters)
            foreach (var root in hierarchyRoots)
            {
                // Apply each linter to all roots
                linter.Lint(root);
            }
        }

        #endregion
    }
}