using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.Audio;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    /// <summary>
    /// A class responsible for handling damage when the train is moving.
    /// </summary>
    public class TrainDamageable : Damageable
    {
        [SerializeField]
        private float _damageInterval;

        private float _baseDamageInterval;

        private protected float p_damageIntervalElapsed;
        private bool _isTrainMoving;

        private void Awake() => _baseDamageInterval = _damageInterval;

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
            PlayRepairSound();

            p_damageIntervalElapsed = 0;
        }

        [ObserversRpc(RunLocally = true)]
        private void PlayRepairSound() 
            => AudioSystem.Instance.PlayRandomSoundEffectOfType(AudioCollectionTypes.Repair, true, .5f);

        /// <summary>
        /// A function to apply the ChaosMultiplier to the <see cref="_damageInterval"/> of this damageable.
        /// </summary>
        /// <param name="chaosMultiplier">The multiplier of how much faster the damage must occur.</param>
        public void ToggleChaosMultiplier(float chaosMultiplier, bool isChaos)
        {
            _damageInterval = isChaos
                ? _baseDamageInterval / chaosMultiplier
                : _baseDamageInterval;
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