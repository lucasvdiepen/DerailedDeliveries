using DerailedDeliveries.Framework.Train;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Splines;

namespace DerailedDeliveries.Framework.Gameplay.Map
{
    /// <summary>
    /// A class that is responsible for updating the visuals of the map.
    /// </summary>
    public class MapUpdater : MonoBehaviour
    {
        [SerializeField]
        private TrainController _train;

        private List<Vector3> positions;

        private void Awake()
        {
            foreach (BezierKnot knot in _train.Spline.Splines[0].Knots)
            {
                Vector3 newPos = knot.Position;
                newPos += _train.Spline.gameObject.transform.position;
                positions.Add(newPos);
            }
        }

        private void Update()
        {
            
        }
    }
}