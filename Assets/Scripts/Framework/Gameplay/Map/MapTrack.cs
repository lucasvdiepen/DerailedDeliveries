using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Map
{
    /// <summary>
    /// A class that holds data and functionality for updating the current position on the map.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MapTrack : MonoBehaviour
    {
        /// <summary>
        /// The TrackID of this <see cref="MapTrack"/>.
        /// </summary>
        public int trackID = -1;

        /// <summary>
        /// The <see cref="SpriteRenderer"/> of the bad split warning sprite.
        /// </summary>
        public SpriteRenderer badSplitWarning;

        /// <summary>
        /// The array of <see cref="RectTransform"/>s that is lerped from <see cref="RectTransform"/> to 
        /// <see cref="RectTransform"/> to represent the current Track.
        /// </summary>
        public RectTransform[] mapPath;

        /// <summary>
        /// The sprite renderer of the track sprite.
        /// </summary>
        public SpriteRenderer spriteRenderer;

        private void Awake()
        {
            if(spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}