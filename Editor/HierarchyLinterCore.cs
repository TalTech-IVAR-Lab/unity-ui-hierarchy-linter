using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    [InitializeOnLoad]
    public static class HierarchyLinterCore
    {
        #region Data

        private static readonly List<IUnityUILinter> Linters = new()
        {
            new RectTransformNamingLinter(),
            new RectTransformValuesLinter(),
            new LayoutGroupExclusivityLinter(),
            new UIComponentOrderLinter(),
        };

        #endregion

        #region Lifecycle

        static HierarchyLinterCore()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            RunLinters();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if linters are allowed to run at the moment.
        /// </summary>
        /// <remarks>
        /// Linters are not allowed to run in play mode to avoid extra performance cost.
        /// </remarks>
        /// <returns>True if linters can run, false otherwise.</returns>
        public static bool CanLint()
        {
            // don't lint while playing or paused
            if (EditorApplication.isPlayingOrWillChangePlaymode) return false;
            if (EditorApplication.isPaused) return false;

            return true;
        }

        /// <summary>
        /// Runs all active linters on all UI elements in the current scene.
        /// </summary>
        public static void RunLinters()
        {
            if (!CanLint()) return;

            var rects = StageUtility.GetCurrentStageHandle().FindComponentsOfType<RectTransform>();
            RunLinters(rects);
        }

        /// <summary>
        /// Runs all active linters on all UI elements under the given root.
        /// </summary>
        /// <param name="root">Root object to lint.</param>
        public static void RunLinters(GameObject root)
        {
            if (!CanLint()) return;

            var rects = root.GetComponentsInChildren<RectTransform>(true);
            RunLinters(rects);
        }

        /// <summary>
        /// Runs all active linters on all the given RectTransforms.
        /// </summary>
        /// <param name="rects">RectTransforms to lint.</param>
        private static void RunLinters(RectTransform[] rects)
        {
            if (!CanLint()) return;

            foreach (var linter in Linters)
            foreach (var rect in rects)
            {
                // apply each linter to all UI rects
                linter.Lint(rect);
            }
        }

        #endregion
    }
}