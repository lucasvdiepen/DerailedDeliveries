using FishNet.Object;
using System.Linq;
using UnityEngine;

using DerailedDeliveries.Framework.DamageRepairManagement.Damageables;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for damaging all boxes inside the train which 
    /// are not stored in a shelf when train is riding on a bad split.
    /// </summary>
    [RequireComponent(typeof(TrainController))]
    public class BadRailSplitDamagable : NetworkBehaviour
    {
        private TrainController _trainController;

        [SerializeField]
        private float _damageInterval;

        private BoxDamageable[] boxDamageablesInTrain;

        private bool _isTrainMoving;

        private protected float p_damageIntervalElapsed;

        private void Awake() => _trainController = gameObject.GetComponent<TrainController>();

        private void OnEnable() => _trainController.onRailSplitChange += HandleRailSplitChanged;

        private void OnDisable() => _trainController.onRailSplitChange -= HandleRailSplitChanged;

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

        private void HandleRailSplitChanged(bool badSplit)
        {
            if (!badSplit)
            {
                boxDamageablesInTrain = null;
                return;
            }

            BoxDamageable[] allBoxDamageables = FindObjectsOfType<BoxDamageable>();
            boxDamageablesInTrain = allBoxDamageables.Where(boxDamageable => boxDamageable.IsInTrain).ToArray();
        }

        private void Update()
        {
            if (!IsServer || IsDeinitializing)
                return;

            UpdateTimer();
        }

        [Server]
        private protected virtual void UpdateTimer()
        {
            if (!_isTrainMoving || !_trainController.IsOnBadRailSplit)
                return;

            p_damageIntervalElapsed += Time.deltaTime;

            if (p_damageIntervalElapsed < _damageInterval)
                return;

            p_damageIntervalElapsed = 0;

            for (int i = 0; i < boxDamageablesInTrain.Length; i++)
                boxDamageablesInTrain[i].TakeDamageFromBadSplit();

        }
    }
}
