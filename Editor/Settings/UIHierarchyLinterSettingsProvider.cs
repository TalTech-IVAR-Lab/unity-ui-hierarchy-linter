using UnityEditor;
using UnityEditor.SettingsManagement;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter.Settings
{
    public class UIHierarchyLinterSettingsProvider
    {
        private const string PreferencesPath = "Project/UI Hierarchy Linter";

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new UserSettingsProvider(
                PreferencesPath,
                UIHierarchyLinterSettingsManager.Instance,
                new[] { typeof(UIHierarchyLinterSettingsProvider).Assembly },
                SettingsScope.Project
            );

            return provider;
        }

        [UserSetting("General", "Lint automatically",
            "When enabled, linters are run automatically in all open Scenes")]
        public static UserSetting<bool> LintAutomatically = new(
            UIHierarchyLinterSettingsManager.Instance,
            nameof(LintAutomatically),
            false
        );
    }
}