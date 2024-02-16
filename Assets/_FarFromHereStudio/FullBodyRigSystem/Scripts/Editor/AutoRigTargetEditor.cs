using UnityEditor;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    [CustomEditor(typeof(AutoRigTarget))]
    [CanEditMultipleObjects]
    public class AutoRigTargetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif
