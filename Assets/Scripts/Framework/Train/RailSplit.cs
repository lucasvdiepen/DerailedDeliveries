using UnityEngine;
using UnityEngine.Splines;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for splitting rail track.
    /// </summary>
    public class RailSplit : MonoBehaviour
    {
        /// <summary>
        /// Slots for the different rail tracks.
        /// </summary>
        [field: SerializeField]
        public SplineContainer[] PossibleTracks { get; private set; }

        /// <summary>
        /// Slots for the different rail tracks that have to connect to a seperate child rail track.
        /// </summary>
        [field: SerializeField]
        public SplineContainer[] PossibleReversalTracks { get; private set; }

        private void Awake()
        {
            if (PossibleTracks.Length > 2)
                Debug.LogWarning("This RailSplit contains more than two possible tracks", this);

            if(PossibleTracks.Length < 2)
                Debug.LogWarning("This RailSplit contains less than two possible tracks", this);
        }
    }
}
