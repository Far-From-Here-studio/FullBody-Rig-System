using UnityEngine;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    public class LookAtModule : MonoBehaviour
    {
        public Transform Rotator;
        public Transform AnimationTarget;
        public Transform WorldSpaceTarget;
    }
}
#endif