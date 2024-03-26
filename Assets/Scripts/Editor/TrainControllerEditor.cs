#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using DerailedDeliveries.Framework.Train;

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

        if (!Application.isPlaying)
        {
            if (GUILayout.Button("Recalculate Spline Length"))
            {
                trainController.RecalculateSplineLenght();
                trainController.DebugSnapToSpline();
            }

            if (GUILayout.Button("Reset Train Position"))
                trainController.ResetTrainPosition();

            return;
        }
        
        TrainEngine engineScript = trainController.TrainEngine;
        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.EnumPopup("Current engine state: ", engineScript.EngineState);
        EditorGUILayout.EnumPopup("Current engine speed state: ", engineScript.CurrentEngineSpeedType);

        if(engineScript.CurrentTargetEngineSpeedType != engineScript.CurrentEngineSpeedType)
            EditorGUILayout.EnumPopup("Target engine speed state: ", engineScript.CurrentTargetEngineSpeedType);
            
        EditorGUILayout.FloatField("Current speed: ", engineScript.CurrentVelocity / trainController.SplineLenght);
        EditorGUILayout.FloatField("Current distance along spline: ", trainController.DistanceAlongSpline);

        EditorGUI.EndDisabledGroup();
    }
}
#endif