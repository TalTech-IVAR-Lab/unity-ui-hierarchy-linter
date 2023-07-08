using UnityEditor;
using UnityEditor.SettingsManagement;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter.Settings
{
    public class UIHierarchyLinterSettings
    {
        #region Singleton

        private const string PreferencesPath = "Project/UI Hierarchy Linter";
        private const string PackageName = "ee.taltech.ivar.unity-ui-hierarchy-linter";

        private static UnityEditor.SettingsManagement.Settings _instance;
        private static UnityEditor.SettingsManagement.Settings Instance => _instance ??= new UnityEditor.SettingsManagement.Settings(PackageName);

        #endregion

        #region Initialization

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new UserSettingsProvider(
                PreferencesPath,
                Instance,
                new[] { typeof(UIHierarchyLinterSettings).Assembly },
                SettingsScope.Project
            );

            return provider;
        }

        #endregion

        #region Settings

        [UserSetting("General", "Lint automatically",
            "When enabled, linters are run automatically in all open Scenes")]
        private static UserSetting<bool> LintAutomaticallySetting = new(
            Instance,
            nameof(LintAutomaticallySetting),
            false
        );

        public static bool LintAutomatically => LintAutomaticallySetting.value;

        #endregion
    }
}