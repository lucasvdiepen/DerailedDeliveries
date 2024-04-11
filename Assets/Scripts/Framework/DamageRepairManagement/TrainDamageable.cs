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
            if (!IsServer)
                return;

            UpdateTimer();
        }

        [Server]
        private void UpdateTimer()
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