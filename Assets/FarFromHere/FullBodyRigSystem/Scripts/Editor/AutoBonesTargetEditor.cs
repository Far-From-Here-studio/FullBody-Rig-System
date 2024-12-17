using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if FFH_ANIMATIONPACKAGE

using UnityEngine.Animations.Rigging;
using static FFH.Animation.AutoBonesTarget;

namespace FFH.Animation
{
    [CustomEditor(typeof(AutoBonesTarget))]
    [CanEditMultipleObjects]
    public class AutoBonesTargetEditor : Editor
    {
        public static AutoBonesTarget autoBonesTarget;
        SerializedProperty PinTransfrom;
        SerializedProperty Mode;
        SerializedProperty RigAsset;
        SerializedProperty TimelinePlayable;
        SerializedProperty TimelineMode;
        SerializedProperty HeadTargetWSWeight;
        SerializedProperty LookAtTarget;
        SerializedProperty AnimatedHeadTarget;
        SerializedProperty rigTargets;
        SerializedProperty PinTargets;
        public static RigModules RigModules;

        void OnEnable()
        {
            autoBonesTarget = (AutoBonesTarget)target;
            InitGUI();
        }

        void InitGUI()
        {
            // Fetch the objects from the GameObject script to display in the inspector
            PinTransfrom = serializedObject.FindProperty("pinTransform");
            Mode = serializedObject.FindProperty("Mode");
            RigAsset = serializedObject.FindProperty("RigAsset");
            //TimelinePlayable = serializedObject.FindProperty("playableDirector");
            TimelineMode = serializedObject.FindProperty("TimeLineMode");
            HeadTargetWSWeight = serializedObject.FindProperty("HeadTargetWSWeight");
            LookAtTarget = serializedObject.FindProperty("LookAtTarget");
            AnimatedHeadTarget = serializedObject.FindProperty("AnimatedHeadTarget");
            rigTargets = serializedObject.FindProperty("rigTargets");
            PinTargets = serializedObject.FindProperty("PinTargets");
            RigModules = autoBonesTarget.RigModules;
        }
        public override void OnInspectorGUI()
        {

            //base.OnInspectorGUI();
            EditorGUILayout.PropertyField(PinTransfrom);
            EditorGUILayout.PropertyField(Mode);
            EditorGUILayout.PropertyField(RigAsset);
            //EditorGUILayout.PropertyField(TimelinePlayable);
            EditorGUILayout.PropertyField(TimelineMode);
            EditorGUILayout.PropertyField(rigTargets);
            EditorGUILayout.PropertyField(PinTargets);

            GUIContent buttonUIcontent = new GUIContent() { text = "Open Rig Controller" };
            var openRigModulesWindow = GUILayout.Button(buttonUIcontent);
            if (openRigModulesWindow)
            {
                var RigWindow = RigModulesEditorWindow.Init(autoBonesTarget);
            }
            if (autoBonesTarget.RigModules && autoBonesTarget.RigModules.LookAtModule) LookAtGUI();

            serializedObject.ApplyModifiedProperties();
        }


        private void LookAtGUI()
        {
            if (autoBonesTarget.RigModules.LookAtModule)
            {
                EditorGUILayout.PropertyField(HeadTargetWSWeight);
                EditorGUILayout.PropertyField(LookAtTarget);
                EditorGUILayout.PropertyField(AnimatedHeadTarget);
            }
        }
      

        [MenuItem("Animation Rigging/Auto Setup Human Rig")]
        public static void SetupRigSelection()
        {
            Animator animator = Selection.activeGameObject.GetComponent<Animator>();
            AutoBonesTarget c = autoBonesTarget;
            RigMapper m = Selection.activeGameObject.GetComponent<RigMapper>();

            if(!Selection.activeGameObject.GetComponent<RigBuilder>())
            {
                Selection.activeGameObject.AddComponent<RigBuilder>();
            }

            if (c == null)
            {
                c = Selection.activeGameObject.AddComponent<AutoBonesTarget>();
            }
            SetupRigAvatar(animator, c);
        }

        [MenuItem("Animation Rigging/Pin Transform")]
        public static void PinTransformSelection()
        {
            var c = Selection.activeGameObject.GetComponent<AutoBonesTarget>();
            c.PinTransform();
        }
        // Assign the closest bone to the corresponding bone category
        public static void AssignBoneToBoneCategory(Transform[] bones, string boneCategoryName, out Transform assignedBone)
        {
            assignedBone = null;
            int minDistance = int.MaxValue;

            foreach (var bone in bones)
            {
                int distance = LevenshteinDistance(bone.name.ToLower(), boneCategoryName);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    assignedBone = bone;
                }
            }
        }

        // Levenshtein distance calculation
        private static int LevenshteinDistance(string s1, string s2)
        {
            int[,] distance = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
            {
                for (int j = 0; j <= s2.Length; j++)
                {
                    if (i == 0)
                        distance[i, j] = j;
                    else if (j == 0)
                        distance[i, j] = i;
                    else
                        distance[i, j] = Mathf.Min(Mathf.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + (s1[i - 1] == s2[j - 1] ? 0 : 1));
                }
            }

            return distance[s1.Length, s2.Length];
        }

        public static void SkelletonDefinition(AutoBonesTarget autoBonesTarget, Transform[] bones, DDCSkeletonDefinition RigNameConvention)
        {
            AssignBoneToBoneCategory(bones, RigNameConvention.Root, out autoBonesTarget.Root);
            AssignBoneToBoneCategory(bones, RigNameConvention.Hips, out autoBonesTarget.Hips);
            AssignBoneToBoneCategory(bones, RigNameConvention.Spine_01, out autoBonesTarget.Spine);
            AssignBoneToBoneCategory(bones, RigNameConvention.Spine_02, out autoBonesTarget.Spine1);
            AssignBoneToBoneCategory(bones, RigNameConvention.Spine_03, out autoBonesTarget.Spine2);
            AssignBoneToBoneCategory(bones, RigNameConvention.neck_01, out autoBonesTarget.Neck);
            AssignBoneToBoneCategory(bones, RigNameConvention.head, out autoBonesTarget.Head);

            // Left arm
            AssignBoneToBoneCategory(bones, RigNameConvention.clavicle_l, out autoBonesTarget.LeftShoulder);
            AssignBoneToBoneCategory(bones, RigNameConvention.upperarm_l, out autoBonesTarget.LeftArm);
            AssignBoneToBoneCategory(bones, RigNameConvention.lowerarm_l, out autoBonesTarget.LeftForeArm);
            AssignBoneToBoneCategory(bones, RigNameConvention.hand_l, out autoBonesTarget.LeftHand);

            // Right arm
            AssignBoneToBoneCategory(bones, RigNameConvention.clavicle_r, out autoBonesTarget.RightShoulder);
            AssignBoneToBoneCategory(bones, RigNameConvention.upperarm_r, out autoBonesTarget.RightArm);
            AssignBoneToBoneCategory(bones, RigNameConvention.lowerarm_r, out autoBonesTarget.RightForeArm);
            AssignBoneToBoneCategory(bones, RigNameConvention.hand_r, out autoBonesTarget.RightHand);

            // Left Leg
            AssignBoneToBoneCategory(bones, RigNameConvention.thigh_l, out autoBonesTarget.LeftUpLeg);
            AssignBoneToBoneCategory(bones, RigNameConvention.calf_l, out autoBonesTarget.LeftLeg);
            AssignBoneToBoneCategory(bones, RigNameConvention.foot_l, out autoBonesTarget.LeftFoot);
            AssignBoneToBoneCategory(bones, RigNameConvention.ball_l, out autoBonesTarget.LeftToeBase);

            // Right Leg
            AssignBoneToBoneCategory(bones, RigNameConvention.thigh_r, out autoBonesTarget.RightUpLeg);
            AssignBoneToBoneCategory(bones, RigNameConvention.calf_r, out autoBonesTarget.RightLeg);
            AssignBoneToBoneCategory(bones, RigNameConvention.foot_r, out autoBonesTarget.RightFoot);
            AssignBoneToBoneCategory(bones, RigNameConvention.ball_r, out autoBonesTarget.RightToeBase);
        }
        public static void HumanDefinition(AutoBonesTarget autoBonesTarget, Animator animator)
        {
            autoBonesTarget.Root = animator.avatarRoot;
            autoBonesTarget.Hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            autoBonesTarget.Spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            autoBonesTarget.Spine1 = autoBonesTarget.Spine.transform.GetChild(0);
            autoBonesTarget.Spine2 = autoBonesTarget.Spine1.transform.GetChild(0);
            autoBonesTarget.Neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            autoBonesTarget.Head = animator.GetBoneTransform(HumanBodyBones.Head);

            autoBonesTarget.LeftUpLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            autoBonesTarget.LeftLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            autoBonesTarget.LeftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            autoBonesTarget.LeftToeBase = animator.GetBoneTransform(HumanBodyBones.LeftToes);

            autoBonesTarget.RightUpLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            autoBonesTarget.RightLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            autoBonesTarget.RightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
            autoBonesTarget.RightToeBase = animator.GetBoneTransform(HumanBodyBones.RightToes);

            autoBonesTarget.LeftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            autoBonesTarget.LeftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            autoBonesTarget.LeftForeArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            autoBonesTarget.LeftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);

            autoBonesTarget.RightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            autoBonesTarget.RightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            autoBonesTarget.RightForeArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            autoBonesTarget.RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        }
        public static void SetupRigAvatar(Animator animator, AutoBonesTarget autoBonesTarget)
        {
            BoneRenderer boneRenderer = autoBonesTarget.GetComponent<BoneRenderer>();
            if (boneRenderer == null)
            {
                boneRenderer = animator.gameObject.AddComponent<BoneRenderer>();
                BoneRendererSetup(animator.transform);
            }
            var rigBuilder = autoBonesTarget.GetComponent<RigBuilder>();

            switch (autoBonesTarget.Mode)
            {
                case SkeletonModel.MaxCCreator:
                    var Maxbones = boneRenderer.transforms;
                    SkelletonDefinition(autoBonesTarget, Maxbones, autoBonesTarget.MaxCC);
                    break;
                case SkeletonModel.Maya:
                    var Mayabones = boneRenderer.transforms;
                    SkelletonDefinition(autoBonesTarget, Mayabones, autoBonesTarget.Maya);
                    break;
                case SkeletonModel.Mixamo:
                    var Mixamobones = boneRenderer.transforms;
                    SkelletonDefinition(autoBonesTarget, Mixamobones, autoBonesTarget.Mixamo);
                    //autoBonesTarget.Root = animator.transform;
                    break;
                case SkeletonModel.HumanBodyBones:
                    HumanDefinition(autoBonesTarget, animator);
                    break;
            }

            if (autoBonesTarget.RigAsset == null)
            {
                //instantiate in runtime, move it later
                autoBonesTarget.RigAsset = autoBonesTarget.InstantiateRig(autoBonesTarget.RigPrefab, autoBonesTarget.transform);
                autoBonesTarget.RigAsset.name = "MasterRig";
                var mapper = autoBonesTarget.RigAsset.GetComponent<RigMapper>();
                if (!autoBonesTarget.RigModules) autoBonesTarget.RigModules = autoBonesTarget.gameObject.AddComponent<RigModules>();

                // autoBonesTarget.RigModules.FootIKRig = autoBonesTarget.RigAsset.GetComponentInChildren<FootIKModule>();
                // autoBonesTarget.RigModules.UpbodyRig = autoBonesTarget.RigAsset.GetComponentInChildren<UpbodyRigModule>();
                //Get Skeletton Definition
                var rig = mapper.GetComponent<Rig>();
                var layer = rigBuilder.layers;
                var riglayer = new RigLayer(rig, true);
                layer.Add(riglayer);
                autoBonesTarget.rigTargets = animator.GetComponentsInChildren<AutoRigTarget>();
                autoBonesTarget.PinTargets = animator.GetComponentsInChildren<AutoConstraintTarget>();

                SetContraints(autoBonesTarget, mapper);
                autoBonesTarget.PinTransform();
                InitRigModule(autoBonesTarget.RigModules);
            }
            else
            {
                var mapper = autoBonesTarget.RigAsset.GetComponent<RigMapper>();
                if (!autoBonesTarget.RigModules) autoBonesTarget.RigModules = autoBonesTarget.gameObject.AddComponent<RigModules>();

                //Get Skeletton Definition
                var rig = mapper.GetComponent<Rig>();
                var layer = rigBuilder.layers;
                var riglayer = new RigLayer(rig, true);
                layer.Add(riglayer);
                autoBonesTarget.rigTargets = animator.GetComponentsInChildren<AutoRigTarget>();
                autoBonesTarget.PinTargets = animator.GetComponentsInChildren<AutoConstraintTarget>();

                SetContraints(autoBonesTarget, mapper);
                autoBonesTarget.PinTransform();
                InitRigModule(autoBonesTarget.RigModules);
            }
        }
        private static void InitRigModule(RigModules rigModules)
        {

            rigModules.upbodyModule = autoBonesTarget.GetComponentInChildren<UpbodyModule>();
            rigModules.UpbodyRig = rigModules.upbodyModule.GetComponent<Rig>();
            rigModules.lookAtModule = autoBonesTarget.GetComponentInChildren<LookAtModule>();
            rigModules.LookAtRig = rigModules.lookAtModule.GetComponent<Rig>();
            rigModules.footIKModule = autoBonesTarget.GetComponentInChildren<FootIKModule>();
            rigModules.FootIKRig = rigModules.footIKModule.GetComponent<Rig>();

            rigModules.ragDollRig = autoBonesTarget.GetComponentInChildren<RagdollModule>();
            rigModules.RagdollRig = rigModules.ragDollRig.FullbodyRagdollRig;
            var RagdollSetup = autoBonesTarget.gameObject.AddComponent<RagDollRig>();

            RagdollSetup.MainRig = autoBonesTarget.RigAsset.GetComponent<Rig>();
            RagdollSetup.root = autoBonesTarget.gameObject.transform;
            RagdollSetup.hips = rigModules.ragDollRig.FullbodyRagdollRig.transform;
            RagdollSetup.FullBodyRagDollRig = rigModules.RagdollRig;
            RagdollSetup.RagdollLeftFoot = rigModules.ragDollRig.RagdollLeftFoot;
            RagdollSetup.RagdollRightFoot = rigModules.ragDollRig.RagdollRightFoot;

            if (!rigModules.lookAtModule.WorldSpaceTarget)
            {
                var newtarget = autoBonesTarget.InstantiateEmpty(autoBonesTarget.gameObject.name + "_LookAtTarget", null).transform;
                rigModules.lookAtModule.WorldSpaceTarget = newtarget.transform;
            }
            if (!rigModules.lookAtModule.AnimationTarget)
            {
                var _newtarget = autoBonesTarget.InstantiateEmpty("AnimatedHeadTarget", autoBonesTarget.RigAsset.transform);
                _newtarget.AddComponent<RigTransform>();
                rigModules.lookAtModule.AnimationTarget = _newtarget.transform;
            }

            var blendHeadTarget = autoBonesTarget.InstantiateRig(autoBonesTarget.BlendHeadTarget, autoBonesTarget.transform);
            blendHeadTarget.name = autoBonesTarget.BlendHeadTarget.name;
            rigModules.BlendHeadTarget = blendHeadTarget.GetComponent<Rig>();
            rigModules.BlendHeadTarget.GetComponent<BlendConstraint>().data.sourceObjectA = autoBonesTarget.RigModules.lookAtModule.AnimationTarget;
            rigModules.BlendHeadTarget.GetComponent<BlendConstraint>().data.sourceObjectB = autoBonesTarget.RigModules.lookAtModule.WorldSpaceTarget.transform;


            WeightedTransform weightedTransform = new WeightedTransform() { transform = blendHeadTarget.transform, weight = 1 };

            if (rigModules.upbodyModule.Spine1.TryGetComponent<MultiAimConstraint>(out var hipsaimConstraint))
            {
                hipsaimConstraint.data.sourceObjects = new WeightedTransformArray() { weightedTransform };
            }
            if (rigModules.upbodyModule.Spine1.TryGetComponent<MultiAimConstraint>(out var SpineAimConstraint))
            {
                SpineAimConstraint.data.sourceObjects = new WeightedTransformArray() { weightedTransform };
            }
            if (rigModules.lookAtModule.Rotator.TryGetComponent<MultiAimConstraint>(out var HeadAimConstraint))
            {
                HeadAimConstraint.data.sourceObjects = new WeightedTransformArray() { weightedTransform };
            }
            //LOOK AT BINDING
            autoBonesTarget.LookAtTarget = blendHeadTarget;

        }
        public static void SetContraints(AutoBonesTarget autoBonesTarget, RigMapper mapper)
        {
            var constraints = autoBonesTarget.GetComponentsInChildren<AutoConstraintTarget>();
            foreach (var constraint in constraints)
            {
                var multiPos = new MultiPositionConstraint();
                var multiRotation = new MultiRotationConstraint();
                var multiParent = new MultiParentConstraint();
                var overrideTransform = new OverrideTransform();

                switch (constraint.target)
                {
                    case ConstraintTarget.Root:
                        // Code for Root constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.Root;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.Root;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.Root;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.Root;
                        }
                        break;
                    case ConstraintTarget.Hips:
                        // Code for Hips constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.Hips;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.Hips;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.Hips;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.Hips;
                        }
                        break;
                    case ConstraintTarget.Spine:
                        // Code for Spine constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.Spine;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.Spine;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.Spine;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.Spine;
                        }
                        break;
                    case ConstraintTarget.Spine1:
                        // Code for Spine1 constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.Spine1;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.Spine1;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.Spine1;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.Spine1;
                        }
                        break;
                    case ConstraintTarget.Spine2:
                        // Code for Spine2 constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.Spine2;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.Spine2;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.Spine2;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.Spine2;
                        }
                        break;
                    case ConstraintTarget.Neck:
                        // Code for Neck constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.Neck;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.Neck;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.Neck;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.Neck;
                        }
                        break;
                    case ConstraintTarget.Head:
                        // Code for Head constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.Head;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.Head;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.Head;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.Head;
                        }
                        break;
                    case ConstraintTarget.RightShoulder:
                        // Code for RightShoulder constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.RightShoulder;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.RightShoulder;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.RightShoulder;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.RightShoulder;
                        }
                        break;
                    case ConstraintTarget.RightArm:
                        // Code for RightArm constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.RightArm;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.RightArm;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.RightArm;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.RightArm;
                        }

                        break;
                    case ConstraintTarget.RightForeArm:
                        // Code for RightForeArm constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.RightForeArm;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.RightForeArm;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.RightForeArm;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.RightForeArm;
                        }
                        break;
                    case ConstraintTarget.RightHand:
                        // Code for RightHand constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.RightHand;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.RightHand;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.RightHand;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.RightHand;
                        }
                        break;
                    case ConstraintTarget.LeftShoulder:
                        // Code for LeftShoulder constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.LeftShoulder;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.LeftShoulder;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.LeftShoulder;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.LeftShoulder;
                        }
                        break;
                    case ConstraintTarget.LeftArm:
                        // Code for LeftArm constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.LeftArm;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.LeftArm;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.LeftArm;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.LeftArm;
                        }
                        break;
                    case ConstraintTarget.LeftForeArm:
                        // Code for LeftForeArm constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.LeftForeArm;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.LeftForeArm;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.LeftForeArm;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.LeftForeArm;
                        }
                        break;
                    case ConstraintTarget.LeftHand:
                        // Code for LeftHand constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.LeftHand;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.LeftHand;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.LeftHand;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.LeftHand;
                        }
                        break;
                    case ConstraintTarget.RightUpLeg:
                        // Code for RightUpLeg constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.RightUpLeg;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.RightUpLeg;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.RightUpLeg;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.RightUpLeg;
                        }
                        break;
                    case ConstraintTarget.RightLeg:
                        // Code for RightLeg constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.RightLeg;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.RightLeg;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.RightLeg;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.RightLeg;
                        }
                        break;
                    case ConstraintTarget.RightFoot:
                        // Code for RightFoot constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.RightFoot;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.RightFoot;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.RightFoot;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.RightFoot;
                        }
                        break;
                    case ConstraintTarget.LeftUpLeg:
                        // Code for LeftUpLeg constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.LeftUpLeg;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.LeftUpLeg;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.LeftUpLeg;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.LeftUpLeg;
                        }
                        break;
                    case ConstraintTarget.LeftLeg:
                        // Code for LeftLeg constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.LeftLeg;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.LeftLeg;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.LeftLeg;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.LeftLeg;
                        }
                        break;
                    case ConstraintTarget.LeftFoot:
                        // Code for LeftFoot constraint
                        if (constraint.TryGetComponent<MultiPositionConstraint>(out multiPos))
                        {
                            multiPos.data.constrainedObject = autoBonesTarget.LeftFoot;
                        }
                        if (constraint.TryGetComponent<MultiRotationConstraint>(out multiRotation))
                        {
                            multiRotation.data.constrainedObject = autoBonesTarget.LeftFoot;
                        }
                        if (constraint.TryGetComponent<MultiParentConstraint>(out multiParent))
                        {
                            multiParent.data.constrainedObject = autoBonesTarget.LeftFoot;
                        }
                        if (constraint.TryGetComponent<OverrideTransform>(out overrideTransform))
                        {
                            overrideTransform.data.constrainedObject = autoBonesTarget.LeftFoot;
                        }
                        break;
                    default:
                        // Default case (handle unexpected enum values)
                        break;
                }
            }
            autoBonesTarget.SetContrainedTargetTwoBonesIK(mapper.LeftHand_IK, autoBonesTarget.LeftHand, autoBonesTarget.LeftForeArm, autoBonesTarget.LeftArm);
            autoBonesTarget.SetContrainedTargetTwoBonesIK(mapper.RightHand_IK, autoBonesTarget.RightHand, autoBonesTarget.RightForeArm, autoBonesTarget.RightArm);
            autoBonesTarget.SetContrainedTargetTwoBonesIK(mapper.RightFoot_IK, autoBonesTarget.RightFoot, autoBonesTarget.RightLeg, autoBonesTarget.RightUpLeg);
            autoBonesTarget.SetContrainedTargetTwoBonesIK(mapper.LeftFoot_IK, autoBonesTarget.LeftFoot, autoBonesTarget.LeftLeg, autoBonesTarget.LeftUpLeg);
        }
        public static void BoneRendererSetup(Transform transform)
        {
            var boneRenderer = transform.GetComponent<BoneRenderer>();
            if (boneRenderer == null)
                boneRenderer = Undo.AddComponent<BoneRenderer>(transform.gameObject);
            else
                Undo.RecordObject(boneRenderer, "Bone renderer setup.");

            var animator = transform.GetComponent<Animator>();
            var renderers = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
            var bones = new List<Transform>();
            if (animator != null && renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; ++i)
                {
                    var renderer = renderers[i];
                    for (int j = 0; j < renderer.bones.Length; ++j)
                    {
                        var bone = renderer.bones[j];
                        if (!bones.Contains(bone))
                        {
                            bones.Add(bone);

                            for (int k = 0; k < bone.childCount; k++)
                            {
                                if (!bones.Contains(bone.GetChild(k)))
                                    bones.Add(bone.GetChild(k));
                            }
                        }
                    }
                }
            }
            else
            {
                bones.AddRange(transform.GetComponentsInChildren<Transform>());
            }

            boneRenderer.transforms = bones.ToArray();

            if (PrefabUtility.IsPartOfPrefabInstance(boneRenderer))
                EditorUtility.SetDirty(boneRenderer);
        }
    }
}
#endif