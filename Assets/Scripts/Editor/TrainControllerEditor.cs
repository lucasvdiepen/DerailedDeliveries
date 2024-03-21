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

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.EnumPopup("Current Train Engine State: ", trainController.EngineState);
        EditorGUI.EndDisabledGroup();
    }
}