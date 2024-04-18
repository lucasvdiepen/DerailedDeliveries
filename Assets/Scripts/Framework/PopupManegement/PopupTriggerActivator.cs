using UnityEngine;

using DerailedDeliveries.Framework.PlayerManagement;
using DerailedDeliveries.Framework.TriggerArea;

namespace DerailedDeliveries.Framework.PopupManagement
{
    /// <summary>
    /// A class that is responsible for showing and hiding a popup when the player is in range.
    /// </summary>
    public class PopupTriggerActivator<T> : TriggerArea<T> where T : Component
    {
        [SerializeField]
        private Popup[] _popups;

        private void OnEnable() => OnColliderChange += OnColliderChanged;

        private void OnDisable() => OnColliderChange -= OnColliderChanged;

        private void OnColliderChanged(T[] players)
        {
            if (players.Length == 0)
            {
                ClosePopups();
                return;
            }

            ShowPopups();
        }

        private void ShowPopups()
        {
            foreach(var popup in _popups)
                popup.Show();
        }

        private void ClosePopups()
        {
            foreach (var popup in _popups)
                popup.Close();
        }
    }
}