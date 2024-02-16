using UnityEngine;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    [ExecuteInEditMode]
    public class AutoLookAtTarget : MonoBehaviour
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
