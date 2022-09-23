namespace EE.TalTech.IVAR.UnityUIHierarchyLinter
{
    using UnityEngine;

    /// <summary>
    /// Base interface for classes that lint UI hierarchies.
    /// </summary>
    public interface IUnityUILinter
    {
        public void Lint(RectTransform rect);
    }
}