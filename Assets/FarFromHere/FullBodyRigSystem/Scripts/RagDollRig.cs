using UnityEngine;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    using UnityEngine.Animations.Rigging;
    using UnityEngine.AI;

    public class RagDollRig : MonoBehaviour
    {
        public bool IsPlayer;
        public Rig MainRig;
        [Range(0, 1)]
        public float RagdollWeight;
        public Rig FullBodyRagDollRig;
        public bool DisableAIController;

        public float SnapRagdollTimer = 1f;

        [HideInInspector]
        public Transform hips;
        [HideInInspector]
        public Transform root;
        [HideInInspector]
        public Transform RagdollLeftFoot;
        [HideInInspector]
        public Transform RagdollRightFoot;

        private bool isAgent;
        private bool SnappedRagdoll = false;
        private Vector3 RadollHipsSnapTranform;
        private Vector3 RootSnapTranform;
        private Animator _animator;
        private AutoBonesTarget _autoBonesTarget;
        private BlendConstraint[] _blendConstraints;
        private NavMeshAgent agent;

        private Rigidbody[] ragdollBones;
        private Vector3 LeftFootPos;
        private Vector3 RightFootPos;
        private Quaternion LeftFootRot;
        private Quaternion RightFootRot;
        private CharacterController _charactercontroller;

        bool ResetRagdollToRig;

        public float UpbodyRigweight { get; private set; }

        private float LookAtRigweight;
        private float LookAtSpineweight;
        private float LookAtHipsweight;
        private float FootIKRigweight;
        private float BlendHeadTargetweight;

        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            _autoBonesTarget = GetComponent<AutoBonesTarget>();
            if (IsPlayer)
            {
                _charactercontroller = GetComponent<CharacterController>();
            }
            isAgent = TryGetComponent<NavMeshAgent>(out agent);
            ragdollBones = GetComponentsInChildren<Rigidbody>();
            ResetRagdollToRig = false;
        }

        private void LateUpdate()
        {

            if (!SnappedRagdoll)
            {

            }


            foreach (Rigidbody r in ragdollBones)
            {
                if (!SnappedRagdoll)
                {
                    r.isKinematic = false;
                }
                else
                {
                    r.isKinematic = true;
                    r.Sleep();
                }

            }

            SetRagdollRigWeight();
        }

        //Gradually change the state of the ragdoll from 0 to 1
        //Made to be enabled using an Animation curve or something like that
        //produce nice falling result if curve speed is beetwen 0.5sec and 1sec
        void SetRagdollRigWeight()
        {

            if (RagdollWeight == 0)
            {
                if (!ResetRagdollToRig)
                {
                    UpbodyRigweight = _autoBonesTarget.RigModules.UpbodyRig.weight;
                    LookAtRigweight = _autoBonesTarget.RigModules.LookAtRig.weight;
                    FootIKRigweight = _autoBonesTarget.RigModules.FootIKRig.weight;
                    BlendHeadTargetweight = _autoBonesTarget.RigModules.BlendHeadTarget.weight;

                }
                else
                {
                    foreach (Rigidbody r in ragdollBones)
                    {
                        r.Sleep();
                    }
                   
                    
                    _autoBonesTarget.RigModules.UpbodyRig.weight = UpbodyRigweight;
                    _autoBonesTarget.RigModules.LookAtRig.weight = LookAtRigweight;
                    _autoBonesTarget.RigModules.FootIKRig.weight = FootIKRigweight;
                    _autoBonesTarget.RigModules.BlendHeadTarget.weight = BlendHeadTargetweight;
                }
                ResetRagdollToRig = false;
                if(_autoBonesTarget.pinTransform) { _autoBonesTarget.PinTransform(); }
                FullBodyRagDollRig.weight = 0;

                _animator.SetBool("Dead", false);
                SnappedRagdoll = true;
            }
            else if (RagdollWeight < 0.1f)
            {
                _autoBonesTarget.RigModules.UpbodyRig.weight = 0;
                _autoBonesTarget.RigModules.LookAtRig.weight = 0;
                // _autoBonesTarget.RigModules.LookAtSpine.weight = 0;
                // _autoBonesTarget.RigModules.LookAtHips.weight = 0;
                _autoBonesTarget.RigModules.FootIKRig.weight = 0;
                _autoBonesTarget.RigModules.BlendHeadTarget.weight = 0;


                _animator.SetBool("Dead", true);
                FullBodyRagDollRig.gameObject.SetActive(true);
                FullBodyRagDollRig.weight = Mathf.Clamp01(RagdollWeight);
                // if (isAgent && DisableAIController) agent.enabled = false;
                SnappedRagdoll = false;
                ResetRagdollToRig = true;

            }
            else if (RagdollWeight > 0.1f && RagdollWeight < 0.9f)
            {

                _autoBonesTarget.RigModules.UpbodyRig.weight = 0;
                _autoBonesTarget.RigModules.LookAtRig.weight = 0;
                _autoBonesTarget.RigModules.FootIKRig.weight = 0;
                _autoBonesTarget.RigModules.BlendHeadTarget.weight = 0;

                FullBodyRagDollRig.gameObject.SetActive(true);
                FullBodyRagDollRig.weight = Mathf.Clamp01(RagdollWeight);

                _animator.SetBool("Dead", true);
                ResetRagdollToRig = true;
                SnappedRagdoll = false;


            }
            else if (RagdollWeight > 0.9f)
            {
                ResetRagdollToRig = true;
                FullBodyRagDollRig.weight = Mathf.Clamp01(RagdollWeight);

                _autoBonesTarget.RigModules.UpbodyRig.weight = 0;
                _autoBonesTarget.RigModules.LookAtRig.weight = 0;

                _autoBonesTarget.RigModules.FootIKRig.weight = 0;
                _autoBonesTarget.RigModules.BlendHeadTarget.weight = 0;
                SnappedRagdoll = false;
                _animator.SetBool("Dead", true);

            }
        }
        private void SyncHips(Vector3 HipsPos)
        {
            transform.position = HipsPos;
            root.position = RootSnapTranform;
            hips.position = RadollHipsSnapTranform;
        }
    }
}
#endif