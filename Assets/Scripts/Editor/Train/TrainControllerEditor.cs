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

            EditorGUILayout.FloatField("Current speed: ", trainEngine.CurrentSpeed);
            EditorGUILayout.EnumPopup("Current engine state: ", trainEngine.EngineState);

            EditorGUILayout.Space();

            EditorGUILayout.FloatField("Current engine index: ", trainEngine.CurrentGearIndex);
            EditorGUILayout.FloatField("Current engine gear: ", trainEngine.CurrentEngineAcceleration);

            EditorGUILayout.Space();

            EditorGUILayout.FloatField("Current distance along spline: ", trainController.DistanceAlongSpline);
            EditorGUILayout.FloatField("Current optimal start point: ", trainController.CurrentOptimalStartPoint);
            
            EditorGUILayout.Toggle("Current chosen track upcomming rail split: ", trainController.TrainEngine.CurrentSplitDirection);

            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif