using UnityEngine;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    [System.Serializable]
    public enum ConstraintTarget
    {
        Root = 0,
        Hips = 1,
        Spine = 2,
        Spine1 = 3,
        Spine2 = 4,
        Neck = 5,
        Head = 6,
        RightShoulder = 7,
        RightArm = 8,
        RightForeArm = 9,
        RightHand = 10,
        LeftShoulder = 11,
        LeftArm = 12,
        LeftForeArm = 13,
        LeftHand = 14,
        RightUpLeg = 15,
        RightLeg = 16,
        RightFoot = 17,
        LeftUpLeg = 18,
        LeftLeg = 19,
        LeftFoot = 20
    }

    public class AutoConstraintTarget : MonoBehaviour
    {
        public ConstraintTarget target;
    }
}
#endif