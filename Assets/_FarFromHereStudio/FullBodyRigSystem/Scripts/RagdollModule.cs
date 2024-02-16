using UnityEngine;

#if FFH_ANIMATIONPACKAGE
using UnityEngine.Animations.Rigging;
namespace FFH.Animation
{
    public class RagdollModule : MonoBehaviour
    {
        public Rig FullbodyRagdollRig;
        public Transform RagdollLeftFoot;
        public Transform RagdollRightFoot;
    }
}
    #endif