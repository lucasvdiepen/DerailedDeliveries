using FishNet.Object;
using System;
using UnityEngine;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    /// <summary>
    /// A class responsible for handling damage.
    /// </summary>
    public class Damageable : NetworkBehaviour
    {
        /// <summary>
        /// Invoked when the health is changed.
        /// </summary>
        public Action<int> OnHealthChanged;

        [SerializeField]
        private int _maxHealth;

        [SerializeField]
        private bool _canBeBelowZero;

        /// <summary>
        /// Whether this <see cref="Damageable"/> can take damage.
        /// </summary>
        public bool CanTakeDamage { get; set; } = true;

        /// <summary>
        /// Gets the current health.
        /// </summary>
        public int Health => _health;

        private protected int MaxHealth => _maxHealth;

        private int _health;

        private protected virtual void OnEnable() => _health = _maxHealth;

        [Server]
        private protected virtual void TakeDamage()
        {
            if(!CanTakeDamage)
                return;

            if (_health <= 0 && !_canBeBelowZero)
                return;

            ChangeHealth(_health - 1);
        }

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        private protected void ChangeHealth(int newHealth)
        {
            _health = newHealth;
            OnHealthChanged?.Invoke(_health);
        }
    }
}