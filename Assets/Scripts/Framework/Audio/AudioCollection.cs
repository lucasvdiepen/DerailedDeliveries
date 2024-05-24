using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Audio
{
    /// <summary>
    /// Class responsible for holding data of an audio collection.
    /// </summary>
    [System.Serializable]
    public class AudioCollection
    {
        /// <summary>
        /// Enum type of audio collection.
        /// </summary>
        [field: SerializeField]
        public AudioCollectionTypes AudioCollectionType { get; private set; }

        /// <summary>
        /// List of audio clips of specified collection type.
        /// </summary>
        public List<AudioClip> audioClipList = new();
    }
}
