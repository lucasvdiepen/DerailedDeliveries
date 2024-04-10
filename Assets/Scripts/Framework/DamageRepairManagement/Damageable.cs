using FishNet.Object;
using System;
using UnityEngine;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    public class Damageable : NetworkBehaviour
    {
        public Action<int> OnHealthChanged;

        [SerializeField]
        private int _maxHealth;

        [SerializeField]
        private bool _canBeBelowZero;

        [field: SerializeField]
        public bool CanTakeDamage { get; set; } = true;

        private protected int MaxHealth => _maxHealth;

        public int Health => _health;

        [SerializeField]
        private int _health;

        private protected virtual void OnEnable() => _health = _maxHealth;

        [ServerRpc(RequireOwnership = false)]
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