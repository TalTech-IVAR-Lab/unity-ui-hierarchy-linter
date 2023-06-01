using System.Collections.Generic;
using UnityEditor;
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

        private static void RunLinters()
        {
            // only lint in Edit mode
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused) return;

            var rects = Object.FindObjectsOfType<RectTransform>(true);

            foreach (var linter in Linters)
            foreach (var rect in rects)
            {
                // don't lint in prefabs
                if (PrefabUtility.IsPartOfRegularPrefab(rect)) continue;

                // apply each linter to all UI rects
                linter.Lint(rect);
            }
        }

        #endregion
    }
}