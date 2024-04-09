using FishNet.Object;
using System;
using UnityEngine;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    public class Damageable : NetworkBehaviour, IRepairable
    {
        public Action<int> OnHealthChanged;

        [SerializeField]
        private int _maxHealth;

        public bool CanTakeDamage { get; set; } = true;

        private int _health;

        private protected virtual void OnEnable() => _health = _maxHealth;

        [ServerRpc(RequireOwnership = false)]
        private protected virtual void TakeDamage()
        {
            if(!CanTakeDamage)
                return;

            ChangeHealth(_health - 1);
        }

        [ServerRpc(RequireOwnership = false)]
        public virtual void Repair()
        {
            ChangeHealth(_maxHealth);
        }

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        private protected void ChangeHealth(int newHealth)
        {
            _health = newHealth;
            OnHealthChanged?.Invoke(_health);
        }
    }
}