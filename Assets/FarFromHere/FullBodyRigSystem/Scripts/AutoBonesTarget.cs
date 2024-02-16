using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if FFH_ANIMATIONPACKAGE
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using UnityEditor.Timeline;

namespace FFH.Animation
{
    [ExecuteAlways]
    public class AutoBonesTarget : MonoBehaviour
    {
        public enum SkeletonModel
        {
            HumanBodyBones,
            MaxCCreator,
            Maya,
            Mixamo
        }

        public bool pinTransform;
        public bool TimeLineMode;
        private Vector3 lookAtTargetPos;
        public SkeletonModel Mode;
        public bool UseJob;
        public DDCSkeletonDefinition MaxCC;
        public DDCSkeletonDefinition Maya;
        public DDCSkeletonDefinition Mixamo;
        public GameObject RigPrefab;
        public GameObject RigAsset;
        public GameObject LookAtSpine;
        public GameObject LookAtHips;
        public GameObject BlendHeadTarget;
        public GameObject AnimatedHeadTarget;
        public GameObject LookAtTarget;
        public RigModules RigModules;
        public AutoRigTarget[] rigTargets;
        public AutoConstraintTarget[] PinTargets;
        public float HeadTargetWSWeight;
        public PlayableDirector playableDirector;
        bool deferredEvaluate;
        public RagDollRig ragdollrig;
        private Animator _animator;
        LookAtTarget lookAt;
        #region skelettondefinition
        // Skelleton Bones
      
        public Transform Root;
      
        public Transform Hips;
      
        public Transform Spine;
      
        public Transform Spine1;
      
        public Transform Spine2;
      
        public Transform Neck;
      
        public Transform Head;
      
        public Transform LeftShoulder;
      
        public Transform LeftArm;
      
        public Transform LeftForeArm;
      
        public Transform LeftHand;
      
        public Transform RightShoulder;
      
        public Transform RightArm;
      
        public Transform RightForeArm;
      
        public Transform RightHand;
      
        public Transform LeftUpLeg;
      
        public Transform LeftLeg;
      
        public Transform LeftFoot;
      
        public Transform LeftToeBase;
      
        public Transform RightUpLeg;
      
        public Transform RightLeg;
      
        public Transform RightFoot;
      
        public Transform RightToeBase;
        #endregion

        private float TimelineTime;
        private void OnEnable()
        {
            rigTargets = GetComponentsInChildren<AutoRigTarget>();
            PinTargets = GetComponentsInChildren<AutoConstraintTarget>();
            _animator = GetComponent<Animator>();

            if(!RigModules) RigModules = GetComponent<RigModules>();
            ragdollrig = GetComponent<RagDollRig>();

#if UNITY_EDITOR

            PinTransform();
#endif
        }

        private void LateUpdate()
        {

            /*
            AvatarIKGoal leftfoot = AvatarIKGoal.LeftFoot;
            AvatarIKGoal rightfoot = AvatarIKGoal.RightFoot;
            Vector3 LeftfootPos = _animator.GetIKPosition(leftfoot);
            Vector3 RightfootPos = _animator.GetIKPosition(rightfoot);

            _animator.SetIKPosition(leftfoot, LeftfootPos + RigModules.FootIKRig.transform.localPosition);
            _animator.SetIKPosition(rightfoot, RightfootPos + RigModules.FootIKRig.transform.localPosition);
            */

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                if(ragdollrig) { if (RigModules.RagdollRig.weight == 0 && pinTransform) PinTransform(); }
              
            }
#endif


            playableDirector = TimelineEditor.inspectedDirector;
            if (playableDirector)
            {
                TimeLineMode = true;
            }
            else
            {
                TimeLineMode = false;
                return;
            }

            if (TimeLineMode)
            {
                //don't work!
                //_animator.rootRotation = lookAt.turn();
               
                PinTransform();
                if (TimelineTime != (float)playableDirector.time)
                {
                    
                    TimelineTime = (float)playableDirector.time;
                }
                
                if (RigModules.LookAtModule)
                {
                    LookAtModule(RigModules.BlendHeadTarget.transform);
                }
            }

            }
        private void Update()
        {
            /*
            if (TimeLineMode)
            {
                transform.rotation = lookAt.turn();
            }
            */
        }

#if UNITY_EDITOR
            public void LookAtModule(Transform LookAtTarget)
        {

            if (!EditorApplication.isPlaying)
            {
                if (deferredEvaluate && lookAtTargetPos == LookAtTarget.position)
                {
                    PinTransform();
                    playableDirector.Evaluate();
                    PinTransform();
                    deferredEvaluate = false;
                }
                if (TimeLineMode)
                {
                    if (lookAtTargetPos != LookAtTarget.position)
                    {
                        playableDirector.DeferredEvaluate();
                        PinTransform();
                        deferredEvaluate = true;
                        lookAtTargetPos = LookAtTarget.position;
                    }
                }
            }
        }
#endif

        public void PinTransform()
        {
            foreach (var rigtarget in rigTargets)
            {
                switch (rigtarget.RigTarget)
                {
                    case RigTarget.Root:
                        // Code for Root constraint
                        if (Root && rigtarget.position) rigtarget.transform.position = Root.transform.position;
                        if (Root && rigtarget.rotation) rigtarget.transform.rotation = Root.transform.rotation;
                        break;
                    case RigTarget.Hips:
                        // Code for Hips constraint
                        if(rigtarget.position) rigtarget.transform.position = Hips.transform.position;
                        if(rigtarget.rotation) rigtarget.transform.rotation = Hips.transform.rotation;
                        break;
                    case RigTarget.Spine:
                        // Code for Spine constraint
                        if (rigtarget.position) rigtarget.transform.position = Spine.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = Spine.transform.rotation;
                        break;
                    case RigTarget.Spine1:
                        // Code for Spine1 constraint
                        if (rigtarget.position) rigtarget.transform.position = Spine1.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = Spine1.transform.rotation;
                        break;
                    case RigTarget.Spine2:
                        // Code for Spine2 constraint
                        if (rigtarget.position) rigtarget.transform.position = Spine2.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = Spine2.transform.rotation;
                        break;
                    case RigTarget.Neck:
                        // Code for Neck constraint
                        if (rigtarget.position) rigtarget.transform.position = Neck.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = Neck.transform.rotation;
                        break;
                    case RigTarget.Head:
                        // Code for Head constraint
                        if (rigtarget.position) rigtarget.transform.position = Head.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = Head.transform.rotation;
                        break;
                    case RigTarget.RightShoulder:
                        // Code for RightShoulder constraint
                        if (rigtarget.position) rigtarget.transform.position = RightShoulder.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = RightShoulder.transform.rotation;
                        break;
                    case RigTarget.RightArm:
                        // Code for RightArm constraint
                        if (rigtarget.position) rigtarget.transform.position = RightArm.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = RightArm.transform.rotation;
                        break;
                    case RigTarget.RightForeArm:
                        // Code for RightForeArm constraint
                        if (rigtarget.position) rigtarget.transform.position = RightForeArm.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = RightForeArm.transform.rotation;
                        break;
                    case RigTarget.RightHand:
                        // Code for RightHand constraint
                        if (rigtarget.position) rigtarget.transform.position = RightHand.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = RightHand.transform.rotation;
                        break;
                    case RigTarget.LeftShoulder:
                        // Code for LeftShoulder constraint
                        if (rigtarget.position) rigtarget.transform.position = LeftShoulder.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = LeftShoulder.transform.rotation;
                        break;
                    case RigTarget.LeftArm:
                        // Code for LeftArm constraint
                        if (rigtarget.position) rigtarget.transform.position = LeftArm.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = LeftArm.transform.rotation;
                        break;
                    case RigTarget.LeftForeArm:
                        // Code for LeftForeArm constraint
                        if (rigtarget.position) rigtarget.transform.position = LeftForeArm.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = LeftForeArm.transform.rotation;
                        break;
                    case RigTarget.LeftHand:
                        // Code for LeftHand constraint
                        if (rigtarget.position) rigtarget.transform.position = LeftHand.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = LeftHand.transform.rotation;
                        break;
                    case RigTarget.RightUpLeg:
                        // Code for RightUpLeg constraint
                        if (rigtarget.position) rigtarget.transform.position = RightUpLeg.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = RightUpLeg.transform.rotation;
                        break;
                    case RigTarget.RightLeg:
                        // Code for RightLeg constraint
                        if (rigtarget.position) rigtarget.transform.position = RightLeg.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = RightLeg.transform.rotation;
                        break;
                    case RigTarget.RightFoot:
                        // Code for RightFoot constraint
                        if (rigtarget.position) rigtarget.transform.position = RightFoot.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = RightFoot.transform.rotation;
                        break;
                    case RigTarget.LeftUpLeg:
                        // Code for LeftUpLeg constraint
                        if (rigtarget.position) rigtarget.transform.position = LeftUpLeg.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = LeftUpLeg.transform.rotation;
                        break;
                    case RigTarget.LeftLeg:
                        // Code for LeftLeg constraint
                        if (rigtarget.position) rigtarget.transform.position = LeftLeg.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = LeftLeg.transform.rotation;
                        break;
                    case RigTarget.LeftFoot:
                        // Code for LeftFoot constraint
                        if (rigtarget.position) rigtarget.transform.position = LeftFoot.transform.position;
                        if (rigtarget.rotation) rigtarget.transform.rotation = LeftFoot.transform.rotation;
                        break;
                    default:
                        // Default case (handle unexpected enum values)
                        break;
                }
            }
        }
        public GameObject InstantiateRig(GameObject RigPrefab, Transform root)
        {
            var rig = Instantiate(RigPrefab, root.transform);
            return rig;
        }
        public GameObject InstantiateEmpty(string Name, Transform parent)
        {
            GameObject go = new GameObject(Name);
            go.transform.parent = parent;
            return go;
        }
        public void SetContrainedTargetTwoBonesIK(Transform RigCTRL, Transform Tip, Transform Mid, Transform Root)
        {
            var x = RigCTRL.GetComponent<TwoBoneIKConstraint>();

            if (x != null)
            {
                x.data.tip = Tip;
                x.data.mid = Mid;
                x.data.root = Root;
            }
        }
     
    }
}
#endif