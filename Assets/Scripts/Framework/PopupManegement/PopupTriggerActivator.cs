using UnityEngine;

using DerailedDeliveries.Framework.PlayerManagement;
using DerailedDeliveries.Framework.TriggerArea;

namespace DerailedDeliveries.Framework.PopupManagement
{
    /// <summary>
    /// A class that is responsible for showing and hiding a popup when the player is in range.
    /// </summary>
    [RequireComponent(typeof(Popup))]
    public class PopupTriggerActivator : TriggerArea<PlayerId>
    {
        private Popup _popup;

        private void OnEnable() => OnComponentChange += OnComponentChanged;

        private void OnDisable() => OnComponentChange -= OnComponentChanged;

        private void Awake() => _popup = GetComponentInChildren<Popup>();

        private void OnComponentChanged(PlayerId[] players)
        {
            if (players.Length == 0)
            {
                _popup.Close();
                return;
            }

            _popup.Show();
        }
    }
}