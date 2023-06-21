using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    internal static class UIHierarchyLinterMenuItems
    {
        private const string BaseMenuEntryPath = "GameObject/UI Hierarchy Linter/";

        #region Selection

        private const string LintSelectionEntryPath = BaseMenuEntryPath + "Lint Selection %&l";

        [MenuItem(LintSelectionEntryPath, false, 10)]
        private static void LintSelection()
        {
            var selectedGameObject = Selection.activeGameObject;

            Undo.RegisterFullObjectHierarchyUndo(selectedGameObject, "Lint UI hierarchy for selection");
            UIHierarchyLinterCore.RunLinters(selectedGameObject);

            Debug.Log($"Linted UI hierarchy of {selectedGameObject.name}.", selectedGameObject);
        }

        [MenuItem(LintSelectionEntryPath, true)]
        private static bool ValidateLintSelection()
        {
            if (Selection.activeGameObject == null) return false;
            if (Selection.activeGameObject.GetComponent<RectTransform>() == null) return false;

            return UIHierarchyLinterCore.CanLint();
        }

        #endregion

        #region Whole

        private const string LintWholeEntryPath = BaseMenuEntryPath + "Lint Whole Hierarchy %&#l";

        [MenuItem(LintWholeEntryPath, false, 10)]
        private static void LintWholeHierarchy()
        {
            Undo.IncrementCurrentGroup();

            // register undo for all root objects containing RectTransforms
            var rects = StageUtility.GetCurrentStageHandle().FindComponentsOfType<RectTransform>();
            for (int i = 0; i < rects.Length; i++)
            {
                var rect = rects[i];
                Undo.RegisterCompleteObjectUndo(
                    rect.gameObject,
                    $"Lint whole UI hierarchy (rect {i + 1} out of {rects.Length}, '{rect.name}')"
                );
            }

            UIHierarchyLinterCore.RunLinters();

            Undo.SetCurrentGroupName("Lint whole UI hierarchy");

            Debug.Log($"Linted whole UI hierarchy in the current scene.");
        }

        [MenuItem(LintWholeEntryPath, true)]
        private static bool ValidateLintHierarchy()
        {
            return UIHierarchyLinterCore.CanLint();
        }

        #endregion
    }
}