using UnityEngine;
using Cinemachine;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// Class responsible for holding all necessary station components for easy access.
    /// </summary>
    public class StationCameraBlendingContainer : MonoBehaviour
    {
        /// <summary>
        /// Getter for <see cref="CinemachineVirtualCamera"/> camera attached to this station.
        /// </summary>
        [field: SerializeField]
        public CinemachineVirtualCamera StationCamera { get; private set; }

        /// <summary>
        /// Getter for the bounding box of this station.
        /// </summary>
        [field: SerializeField]
        public BoxCollider StationBoundingBoxCollider { get; private set; }
    }
}
