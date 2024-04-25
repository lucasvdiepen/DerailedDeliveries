using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    /// <summary>
    /// A class responsible for handling damage when the train is moving.
    /// </summary>
    public class TrainDamageable : Damageable
    {
        [SerializeField]
        private float _damageInterval;

        private protected float p_damageIntervalElapsed;
        private bool _isTrainMoving;

        [Server]
        private void OnVelocityChanged(float velocity) => _isTrainMoving = Mathf.Abs(velocity) > 0.1f;

        private void Update()
        {
            if (!IsServer || IsDeinitializing)
                return;

            UpdateTimer();
        }

        [Server]
        private protected virtual void UpdateTimer()
        {
            if (!CanTakeDamage || !_isTrainMoving)
                return;

            p_damageIntervalElapsed += Time.deltaTime;

            if (p_damageIntervalElapsed < _damageInterval)
                return;

            p_damageIntervalElapsed = 0;
            TakeDamage();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [Server]
        public override void Repair()
        {
            base.Repair();

            p_damageIntervalElapsed = 0;
        }

        /// <summary>
        /// A function to apply the ChaosMultiplier to the <see cref="_damageInterval"/> of this damageable.
        /// </summary>
        /// <param name="chaosMultiplier">The multiplier of how much faster the damage must occur.</param>
        public void ApplyChaosMultiplier(float chaosMultiplier) => _damageInterval /= chaosMultiplier;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartClient();

            TrainEngine.Instance.OnSpeedChanged += OnVelocityChanged;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopServer()
        {
            base.OnStopClient();

            if(TrainEngine.Instance == null)
                return;

            TrainEngine.Instance.OnSpeedChanged -= OnVelocityChanged;
        }
    }
}