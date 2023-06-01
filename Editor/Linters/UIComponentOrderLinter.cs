using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    public class UIComponentOrderLinter : IUnityUILinter
    {
        private static readonly List<Type> ComponentTypesOrder = new()
        {
            typeof(RectTransform),

            typeof(Canvas),
            typeof(CanvasGroup),
            typeof(CanvasScaler),

            typeof(LayoutElement),
            typeof(LayoutGroup),

            typeof(Graphic),
        };

        public void Lint(RectTransform rect)
        {
            var components = rect.GetComponents<Component>().ToList();
            var orderedComponents = components.OrderBy(GetComponentOrderValue).ToList();
            orderedComponents.Reverse();

            foreach (var component in orderedComponents)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    // ComponentUtility.MoveComponentUp(component);
                }
                Debug.Log($"Moved {component.GetType().Name} up in hierarchy", component);
            }
        }

        private int GetComponentOrderValue(Component component)
        {
            var componentType = component.GetType();

            if (ComponentTypesOrder.Contains(componentType))
            {
                return ComponentTypesOrder.IndexOf(component.GetType());
            }

            return ComponentTypesOrder.Count;
        }
    }
}