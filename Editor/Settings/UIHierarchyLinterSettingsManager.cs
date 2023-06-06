using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SettingsManagement;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter.Settings
{
    public class UIHierarchyLinterSettingsManager
    {
        private const string PackageName = "ee.taltech.ivar.unity-ui-hierarchy-linter";

        private static UnityEditor.SettingsManagement.Settings _instance;
        
        internal static UnityEditor.SettingsManagement.Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UnityEditor.SettingsManagement.Settings(PackageName);
                }

                return _instance;
            }
        }

        // The rest of this file is just forwarding the various setting methods to the instance.
        
        private const SettingsScope Scope = SettingsScope.Project;
        
        public static void Save()
        {
            _instance.Save();
        }

        public static T Get<T>(string key)
        {
            return _instance.Get<T>(key);
        }

        public static void Set<T>(string key, T value)
        {
            _instance.Set<T>(key, value);
        }

        public static bool ContainsKey<T>(string key)
        {
            return _instance.ContainsKey<T>(key);
        }
    }
}