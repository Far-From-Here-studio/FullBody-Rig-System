using UnityEditor;
using UnityEngine;

public class ScenePhysicsTool : EditorWindow
{

    private void OnGUI()
    {
        if (GUILayout.Button("Run Physics"))
        {
            StepPhysics();
        }
    }
    private void StepPhysics()
    {
        Physics.simulationMode = SimulationMode.Script;
        Physics.Simulate(Time.fixedDeltaTime);
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }

    [MenuItem("FarFromHereStudio/Timeline Utilities/Scene Physics")]
    private static void OpenWindow()
    {
        GetWindow<ScenePhysicsTool>(false, "Physics", true);
    }
}