using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    /// <summary>
    /// Enforces pre-defined order on UI components attached to the linted RectTransform's GameObject.
    /// </summary>
    public class UIComponentOrderLinter : IUnityUILinter
    {
        /// <summary>
        /// List that defines the order in which the corresponding UI component types will be ordered on GameObjects.
        /// </summary>
        /// <remarks>
        /// The types also affect their subclasses (i.e. GridLayoutGroup will be ordered as LayoutGroup).
        /// </remarks>
        private static readonly List<Type> ComponentTypesOrder = new()
        {
            typeof(RectTransform),

            // Canvas components right after RectTransform
            typeof(Canvas),
            typeof(CanvasGroup),
            typeof(CanvasRenderer),
            typeof(CanvasScaler),

            // Layout components stay on top of the rest of UI components
            typeof(LayoutElement),
            typeof(LayoutGroup),
        
            // Other UI components stay at the bottom
            typeof(Graphic),
            typeof(UIBehaviour),
        };

        public void Lint(RectTransform rect)
        {
            var components = rect.GetComponents<Component>().ToList();
            var orderedComponents = components.OrderBy(GetComponentOrderValue).ToList();

            bool didReorderingHappen = false;

            for (int orderedIndex = 0; orderedIndex < orderedComponents.Count; orderedIndex++)
            {
                var component = orderedComponents[orderedIndex];
                int unorderedIndex = components.IndexOf(component);

                if (orderedIndex == unorderedIndex) continue;

                // move the component to the ordered index
                int stepsToMove = unorderedIndex - orderedIndex;
                for (int i = 0; i < stepsToMove; i++) ComponentUtility.MoveComponentUp(component);

                // do the same in the components list
                components.Remove(component);
                components.Insert(orderedIndex, component);

                didReorderingHappen = true;
            }

            if (didReorderingHappen)
                Debug.Log($"UIComponentOrderLinter: Reordered components on {rect.name}.", rect.gameObject);
        }

        private int GetComponentOrderValue(Component component)
        {
            var componentType = component.GetType();

            foreach (var type in ComponentTypesOrder)
            {
                if (IsSameOrSubclassOf(componentType, type)) return ComponentTypesOrder.IndexOf(type);
            }

            return ComponentTypesOrder.Count;
        }

        /// <summary>
        /// Checks if the given type is the same or a subclass of the other type.
        /// </summary>
        /// <param name="thisType">The type to check.</param>
        /// <param name="otherType">The type to check against.</param>
        /// <returns>True if the given type is the same or a subclass of the other type.</returns>
        private static bool IsSameOrSubclassOf(Type thisType, Type otherType)
        {
            return (thisType == otherType) || thisType.IsSubclassOf(otherType);
        }
    }
}