using System;
using UnityEngine;

namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    /// <summary>
    /// Rounds and cleans up transform values of the linted RectTransform.
    /// </summary>
    public class RectTransformValuesLinter: IUnityUILinter
    {
        private const int RoundingPrecision = 6;
        
        public void Lint(RectTransform rect)
        {
            EnforceUnitScale(rect);
            RoundTransformValues(rect);
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
            return (float) Math.Round(value, RoundingPrecision);
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