using UnityEngine;

#if FFH_ANIMATIONPACKAGE
using UnityEngine.Animations.Rigging;

namespace FFH.Animation
{
    public class RigModules : MonoBehaviour
    {
        public bool FootIKModule;
        public FootIKModule footIKModule;
        public Rig FootIKRig;

        public bool UpbodyModule;
        public UpbodyModule upbodyModule;
        public Rig UpbodyRig;

        public bool LookAtModule;
        public LookAtModule lookAtModule;
        public Rig LookAtRig;
        public Rig BlendHeadTarget;

        public bool RagdollModule;
        public RagdollModule ragDollRig;
        public Rig RagdollRig;
    }
}
#endif