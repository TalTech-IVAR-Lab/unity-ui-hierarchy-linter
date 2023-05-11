namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using UnityEngine;

    /// <summary>
    /// Base interface for classes that lint UI hierarchies.
    /// </summary>
    public interface ICanvasLinter
    {
        public void Lint(Canvas canvas);
    }
}