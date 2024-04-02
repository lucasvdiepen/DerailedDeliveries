#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Editor.Train
{
    /// <summary>
    /// Class responsible for drawing debug values in TrainController inspector.
    /// </summary>
    [CustomEditor(typeof(TrainController))]
    public class TrainControllerEditor : UnityEditor.Editor
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
                    trainController.RecalculateSplineLength();
                    trainController.DebugSnapToSpline();
                }

                if (GUILayout.Button("Reset Train Position"))
                    trainController.ResetTrainPosition();

                return;
            }

            TrainEngine trainEngine = trainController.TrainEngine;
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.EnumPopup("Current engine state: ", trainEngine.EngineState);
            EditorGUILayout.EnumPopup("Current engine speed state: ", trainEngine.CurrentEngineSpeedType);

            if (trainEngine.CurrentTargetEngineSpeedType != trainEngine.CurrentEngineSpeedType)
                EditorGUILayout.EnumPopup("Target engine speed state: ", trainEngine.CurrentTargetEngineSpeedType);

            EditorGUILayout.FloatField("Current speed: ", trainEngine.CurrentVelocity);
            EditorGUILayout.FloatField("Current distance along spline: ", trainController.DistanceAlongSpline);
            EditorGUILayout.FloatField("Current optimal start point: ", trainController.CurrentOptimalStartPoint);
            EditorGUILayout.Toggle("Current chosen track upcomming rail split: ", trainController.TrainEngine.CurrentSplitDirection);

            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif