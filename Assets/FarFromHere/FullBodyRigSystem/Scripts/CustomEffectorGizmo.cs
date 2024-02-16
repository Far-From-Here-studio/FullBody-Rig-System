#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FFH.Animation
{
    [ExecuteAlways]
    public class CustomEffectorGizmo : MonoBehaviour
    {
        public bool Unselectable;
        public Mesh gizmoMesh; // Mesh for the gizmo.
        public float gizmoScale = 1f; // Scale for the gizmo.
        public Vector3 gizmoOffset = Vector3.zero; // Offset for the gizmo.
        [HideInInspector]
        public Shader gizmoShader;
        public Color gizmoColor;
        public Vector3 position;
        private Material gizmoMaterial; // Material for the gizmo.

        private void OnEnable()
        {
            Shader shader = Shader.Find("Hidden/RigEffector");
            if (shader != null)
            {
                gizmoMaterial = new Material(shader);
            }
            else
            {
                Debug.LogError("Shader not found!");
            }
        }
        private void OnDrawGizmos()
        {
            if (!Unselectable)
            {
                position = transform.position;
                Gizmos.color = gizmoColor;
                // Draw a handle at the position of the effector.
                // Draw a handle at the position of the effector.
                Gizmos.DrawMesh(gizmoMesh, position, Quaternion.identity, new Vector3(gizmoScale, gizmoScale, gizmoScale));

                // Make the handle selectable.
                if (Event.current.type == EventType.MouseDown)
                {
                    GameObject selectedObject = HandleUtility.PickGameObject(Event.current.mousePosition, true);
                    if (selectedObject == gameObject)
                    {
                        Selection.activeGameObject = selectedObject;
                        Event.current.Use();
                    }
                }
            }

        }

        void OnRenderObject()
        {
            if (EditorApplication.isPlaying) return;
            if (Unselectable)
            {
                if (gizmoMesh == null || gizmoMaterial == null) return;


                gizmoMaterial.SetPass(0); // Set the material's pass
                gizmoMaterial.color = gizmoColor;
                Vector3 scale = new Vector3(gizmoScale, gizmoScale, gizmoScale);
                Graphics.DrawMeshNow(
                    gizmoMesh,
                    Matrix4x4.TRS(transform.position + gizmoOffset, transform.rotation, scale)
                ); // Draw the mesh at the effector's position, with its rotation, and the desired scale and offset.
            }

        }
    }
}

#endif