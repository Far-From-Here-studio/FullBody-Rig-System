#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace FFH.Animation
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomEffectorGizmo))]
    public class CustomEffectorGizmoEditor : Editor
    {
        CustomEffectorGizmo effectorGizmo;
        void OnSceneGUI()
        {
            effectorGizmo = (CustomEffectorGizmo)target;
           
        }
      
    }
}
#endif