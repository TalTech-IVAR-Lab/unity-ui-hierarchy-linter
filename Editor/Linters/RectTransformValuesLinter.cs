using System;
using UnityEngine;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    /// <summary>
    /// Rounds and cleans up transform values of the linted RectTransform.
    /// </summary>
    public class RectTransformValuesLinter : IUnityUILinter
    {
        private const int RoundingPrecision = 6;

        public void Lint(RectTransform rect)
        {
            if (IsWorldSpaceCanvas(rect))
            {
                // do not apply this linter to World Space canvases
                return;
            }

            EnforceUnitScale(rect);
            RoundTransformValues(rect);
        }

        private bool IsWorldSpaceCanvas(RectTransform rect)
        {
            var canvas = rect.GetComponent<Canvas>();
            if (canvas == null) return false;

            return canvas.renderMode == RenderMode.WorldSpace;
        }

        private void EnforceUnitScale(RectTransform rect)
        {
            rect.localScale = Vector3.one;
        }

        private void RoundTransformValues(RectTransform rect)
        {
            rect.position = RoundVector3(rect.position);
            rect.pivot = RoundVector2(rect.pivot);
        }

        private float RoundValue(float value)
        {
            return (float)Math.Round(value, RoundingPrecision);
        }

        private Vector2 RoundVector2(Vector2 vector)
        {
            return new Vector2(
                RoundValue(vector.x),
                RoundValue(vector.y)
            );
        }

        private Vector3 RoundVector3(Vector3 vector)
        {
            return new Vector3(
                RoundValue(vector.x),
                RoundValue(vector.y),
                RoundValue(vector.z)
            );
        }
    }
}