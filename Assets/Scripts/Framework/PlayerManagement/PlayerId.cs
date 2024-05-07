using FishNet.Object;

namespace DerailedDeliveries.Framework.PlayerManagement
{
    public class PlayerId : NetworkBehaviour
    {
        /// <summary>
        /// The unique identifier of the player.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            PlayerManager.Instance.PlayerJoined(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();

            PlayerManager.Instance?.PlayerLeft(this);
        }

        /// <summary>
        /// Sets the unique identifier of the player.
        /// </summary>
        /// <param name="id">The new unique identifier of the player.</param>
        [ObserversRpc(BufferLast = true)]
        public void SetId(int id)
        {
            Id = id;

            PlayerManager.Instance.OnPlayersUpdated?.Invoke();
        }
    }
}