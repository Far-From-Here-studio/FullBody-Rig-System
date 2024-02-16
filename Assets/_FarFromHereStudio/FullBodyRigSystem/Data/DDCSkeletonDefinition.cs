using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFH
{
    [CreateAssetMenu(fileName = "Data", menuName = "FFH/Animation/DDCSkelletonDefinition", order = 1)]
    public class DDCSkeletonDefinition : ScriptableObject
    {
        public string Root;
        public string Hips;
        public string Spine_01;
        public string Spine_02;
        public string Spine_03;
        public string neck_01;
        public string head;

        //Left arm
        public string clavicle_l;
        public string upperarm_l;
        public string lowerarm_l;
        public string hand_l;

        //Right arm
        public string clavicle_r;
        public string upperarm_r;
        public string lowerarm_r;
        public string hand_r;

        //Left Leg
        public string thigh_l;
        public string calf_l;
        public string foot_l;
        public string ball_l;

        //Right Leg
        public string thigh_r;
        public string calf_r;
        public string foot_r;
        public string ball_r;


    }
}