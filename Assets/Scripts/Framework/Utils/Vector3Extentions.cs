using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Utils.Vector3Extentions
{
    /// <summary>
    /// Helper class responsible for adding extra functionality to a Vector3.
    /// </summary>
    public static class Vector3Extentions
    {
        public static Vector3 GetNearest(this Vector3 originPosition, Vector3[] positions)
        {
            Vector3 bestTarget = Vector3.zero;
            float closestDistanceSqr = Mathf.Infinity;

            foreach (Vector3 position in positions)
            {
                // Get the direction to the target.
                Vector3 directionToTarget = position - originPosition;

                // Get the squared distance to the target.
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                // Check if the current time stamp is closer than the previous closest time stamp.
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = position;
                }
            }
            return bestTarget;
        }
    }
}
