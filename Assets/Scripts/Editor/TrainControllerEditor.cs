using UnityEditor;

using DerailedDeliveries.Framework.TrainController;
using UnityEngine;

[CustomEditor(typeof(TrainController))]
public class TrainControllerEditor : Editor
{
    /// <summary>
    /// Force editor to keep refreshing.
    /// </summary>
    /// <returns>True</returns>
    public override bool RequiresConstantRepaint() => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TrainController trainController = (TrainController)target;

        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.EnumPopup("Current engine state: ", trainController.EngineState);
        EditorGUILayout.EnumPopup("Current engine speed state: ", trainController.CurrentEngineSpeedType);

        EditorGUILayout.FloatField("Current speed: ", trainController.CurrentVelocity * 100);
        EditorGUILayout.FloatField("Current distance along spline: ", trainController.DistanceAlongSpline);

        EditorGUI.EndDisabledGroup();
    }
}