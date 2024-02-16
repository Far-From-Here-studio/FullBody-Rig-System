using UnityEngine;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    [ExecuteInEditMode]
    public class HeadTargetSnap : MonoBehaviour
    {
        public AutoBonesTarget autoBonesTarget;
        public Transform Target;
        private void OnEnable()
        {
            autoBonesTarget = GetComponentInParent<AutoBonesTarget>();
        }

    }
}
#endif