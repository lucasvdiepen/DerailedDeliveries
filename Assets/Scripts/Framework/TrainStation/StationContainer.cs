using UnityEngine;
using Cinemachine;

namespace DerailedDeliveries.Framework.TrainStation
{
    /// <summary>
    /// Class responsible for holding all necessary station components for easy access.
    /// </summary>
    public class StationContainer : MonoBehaviour
    {
        /// <summary>
        /// Getter for <see cref="CinemachineVirtualCamera"/> camera attached to this station.
        /// </summary>
        [field: SerializeField]
        public CinemachineVirtualCamera StationCamera { get; private set; }

        /// <summary>
        /// Getter for the left-most transform point of this station
        /// </summary>
        [field: SerializeField]
        public Transform LeftCornerPoint { get; private set; }

        /// <summary>
        /// Getter for the right-most transform point of this station
        /// </summary>
        [field: SerializeField]
        public Transform RightCornerPoint { get; private set; }
    }
}
