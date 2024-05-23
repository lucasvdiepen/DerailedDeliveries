using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.DamageRepairManagement;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for damaging all boxes inside the train which 
    /// are not stored in a shelf when train is riding on a bad split.
    /// </summary>
    [RequireComponent(typeof(TrainController))]
    public class BadRailSplitDamager : NetworkBehaviour
    {
        [SerializeField]
        private float _damageInterval;

        private TrainController _trainController;
       
        private TrainDamageable[] _trainDamageables;

        private bool _isTrainMoving;

        private protected float p_damageIntervalElapsed;

        private void Awake() => _trainController = GetComponent<TrainController>();

        private void OnEnable() => _trainController.OnRailSplitChange += HandleRailSplitChanged;

        private void OnDisable() => _trainController.OnRailSplitChange -= HandleRailSplitChanged;

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

            if (TrainEngine.Instance == null)
                return;

            TrainEngine.Instance.OnSpeedChanged -= OnVelocityChanged;
        }

        [Server]
        private void OnVelocityChanged(float velocity) => _isTrainMoving = Mathf.Abs(velocity) > 0.1f;

        private void HandleRailSplitChanged(bool isBadSplit)
        {
            if (!isBadSplit)
            {
                _trainDamageables = null;
                return;
            }

            _trainDamageables = FindObjectsOfType<TrainDamageable>();
        }

        private void Update()
        {
            if (!IsServer || IsDeinitializing)
                return;

            UpdateTimer();
        }

        [Server]
        private void UpdateTimer()
        {
            if (!_isTrainMoving || !_trainController.IsOnBadRailSplit)
                return;

            p_damageIntervalElapsed += Time.deltaTime;

            if (p_damageIntervalElapsed < _damageInterval)
                return;

            p_damageIntervalElapsed = 0;

            for (int i = 0; i < _trainDamageables.Length; i++)
                _trainDamageables[i].TakeDamage();
        }
    }
}