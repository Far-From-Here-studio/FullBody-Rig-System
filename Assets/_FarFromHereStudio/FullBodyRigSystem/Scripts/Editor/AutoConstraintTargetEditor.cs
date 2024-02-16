using UnityEditor;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    [CustomEditor(typeof(AutoConstraintTarget))]
    [CanEditMultipleObjects]
    public class AutoConstraintTargetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif