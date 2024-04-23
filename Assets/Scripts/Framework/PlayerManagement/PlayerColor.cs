using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.PlayerManagement
{
    /// <summary>
    /// A class responsible for managing the color of a player.
    /// </summary>
    public class PlayerColor : NetworkBehaviour
    {
        [SerializeField]
        private SkinnedMeshRenderer _meshRenderer;

        /// <summary>
        /// The color of the player.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Sets the color of the player.
        /// </summary>
        /// <param name="color">The target color.</param>
        [ObserversRpc(BufferLast = true)]
        public void SetColor(Color color)
        {
            Color = color;

            Material newMaterial = new(_meshRenderer.material)
            {
                color = color
            };

            _meshRenderer.material = newMaterial;
        }
    }
}