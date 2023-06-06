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
        
        
    }
}