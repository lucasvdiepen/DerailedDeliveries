using UnityEditor;

using DerailedDeliveries.Framework.TrainController;
using UnityEngine;

[CustomEditor(typeof(TrainController))]
public class TrainControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrainController trainController = (TrainController)target;

        GUILayout.Space(10);
        // Add a custom button to the inspector
        if (GUILayout.Button("Update Train Position"))
        {
            // Call a method in your script when the button is clicked
            trainController.DebugSnapToSpline();
        }
    }
}
