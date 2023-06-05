using UnityEditor;
using UnityEngine;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    internal static class HierarchyLinterMenuItems
    {
        private const string MenuEntryPath = "GameObject/UI Hierarchy Linter/Lint Selection %&l";

        [MenuItem(MenuEntryPath, false, 10)]
        private static void LintHierarchy()
        {
            var selectedGameObject = Selection.activeGameObject;

            Undo.RegisterFullObjectHierarchyUndo(selectedGameObject, "Lint UI hierarchy");
            HierarchyLinterCore.RunLinters(selectedGameObject);

            Debug.Log($"Linted UI hierarchy of {selectedGameObject.name}.", selectedGameObject);
        }

        [MenuItem(MenuEntryPath, true)]
        private static bool ValidateLintHierarchy()
        {
            if (Selection.activeGameObject == null) return false;
            if (Selection.activeGameObject.GetComponent<RectTransform>() == null) return false;

            return HierarchyLinterCore.CanLint();
        }
    }
}