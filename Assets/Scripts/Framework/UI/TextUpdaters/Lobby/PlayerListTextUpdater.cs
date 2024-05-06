using System.Linq;
using UnityEngine;

using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.UI.TextUpdaters.Lobby
{
    /// <summary>
    /// A <see cref="TextUpdater"/> class which updates the player list text.
    /// </summary>
    public class PlayerListTextUpdater : TextUpdater
    {
        [SerializeField]
        private bool _shouldShowOwningPlayers;

        private void OnEnable()
        {
            PlayerManager.Instance.OnPlayerJoined += PlayerJoined;
            PlayerManager.Instance.OnPlayerLeft += UpdateText;
            UpdateText();
        }

        private void OnDisable()
        {
            if(PlayerManager.Instance == null)
                return;

            PlayerManager.Instance.OnPlayerJoined -= PlayerJoined;
            PlayerManager.Instance.OnPlayerLeft -= UpdateText;
        }

        private void PlayerJoined(PlayerId playerId) => UpdateText();

        private void UpdateText()
        {
            string[] playerIds = PlayerManager.Instance.CurrentPlayers
                .Where(playerId => playerId.IsOwner == _shouldShowOwningPlayers)
                .Select(player => "Player " + (player.Id + 1))
                .ToArray();

            ReplaceTag(string.Join("\n", playerIds));
        }
    }
}