using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    /// <summary>
    /// Ensures that RectTransforms with LayoutGroups don't have any other UI components attached to them. 
    /// </summary>
    public class LayoutGroupExclusivityLinter : IUnityUILinter
    {
        public class LayoutGroupExclusivityTag : MonoBehaviour
        {
        }

        public void Lint(RectTransform rect)
        {
            EnsureLayoutGroupExclusivity(rect);
        }

        private void EnsureLayoutGroupExclusivity(RectTransform rect)
        {
            var layoutGroup = rect.GetComponent<LayoutGroup>();
            if (layoutGroup == null)
            {
                // nothing to ensure if there's no LayoutGroup
                return;
            }

            var components = rect.GetComponents<Component>();
            var illegalComponents = components.Where(
                component => component is not (
                    LayoutGroup
                    or Canvas
                    or CanvasRenderer
                    or CanvasGroup
                    or RectTransform
                    or LayoutGroupExclusivityTag
                    )
            ).ToList();

            // if (illegalComponents.Count > 0)
            // {
            //     Debug.LogWarning($"Object '{rect}' contains {illegalComponents.Count} illegal objects:\n" +
            //                      $"{illegalComponents}", rect);
            // }

            var layoutGroupExclusivityTag = rect.GetComponent<LayoutGroupExclusivityTag>();
            
            // TODO: if layout group was just added, 
            
            var layoutGroupJustAdded = (layoutGroupExclusivityTag == null);
        }
    }
}