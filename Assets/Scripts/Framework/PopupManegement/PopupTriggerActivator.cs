using UnityEngine;

using DerailedDeliveries.Framework.PlayerManagement;
using DerailedDeliveries.Framework.TriggerArea;

namespace DerailedDeliveries.Framework.PopupManagement
{
    /// <summary>
    /// A class that is responsible for showing and hiding a popup when the player is in range.
    /// </summary>
    [RequireComponent(typeof(Popup))]
    public class PopupTriggerActivator<T> : TriggerArea<T> where T : Component
    {
        private Popup _popup;

        private void OnEnable() => OnColliderChange += OnColliderChanged;

        private void OnDisable() => OnColliderChange -= OnColliderChanged;

        private void Awake() => _popup = GetComponentInChildren<Popup>();

        private void OnColliderChanged(T[] players)
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