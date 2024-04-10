using DerailedDeliveries.Framework.Train;
using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    public class TrainDamageable : Damageable
    {
        [SerializeField]
        private float _damageInterval;

        [SerializeField]
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

        public override void OnStartServer()
        {
            base.OnStartClient();

            TrainEngine.Instance.OnSpeedChanged += OnVelocityChanged;
        }

        public override void OnStopServer()
        {
            base.OnStopClient();

            if(TrainEngine.Instance == null)
                return;

            TrainEngine.Instance.OnSpeedChanged -= OnVelocityChanged;
        }
    }
}