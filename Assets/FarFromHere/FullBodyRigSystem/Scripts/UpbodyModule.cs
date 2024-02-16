using UnityEngine;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    public class UpbodyModule : MonoBehaviour
    {
        public Transform Hips;
        public Transform Spine;
        public Transform Spine1;
        public Transform Spine2;
        public Transform Neck;
        public Transform Head;
        public Transform LeftHandIKHint;
        public Transform LeftHandIK;
        public Transform RightHandIKHint;
        public Transform RightHandIK;
    }
}
#endif