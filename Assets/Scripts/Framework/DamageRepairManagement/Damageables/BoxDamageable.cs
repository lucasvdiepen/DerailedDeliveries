using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.DamageRepairManagement.Damageables
{
    /// <summary>
    /// A <see cref="TrainDamageable"/> class which handles logic for the box.
    /// </summary>
    public class BoxDamageable : TrainDamageable
    {
        [SerializeField]
        private LayerMask _trainLayer;

        private int _amountInTrain;

        public bool IsInTrain => _amountInTrain > 0;

        private TrainController _trainController;

        private void Awake() => _trainController = TrainEngine.Instance.GetComponent<TrainController>();

        [Server]
        private protected override void TakeDamage()
        {
            // Take double damage if train is on bad rail split.
            if (_trainController.IsOnBadRailSplit && IsInTrain)
                base.TakeDamage();
            
            if(_amountInTrain == 0)
                return;
            
            base.TakeDamage();
        }

        private void OnTriggerEnter(Collider other)
        {
            if((_trainLayer.value & 1 << other.gameObject.layer) == 0)
                return;

            _amountInTrain++;
        }

        private void OnTriggerExit(Collider other)
        {
            if((_trainLayer.value & 1 << other.gameObject.layer) == 0)
                return;

            _amountInTrain--;
        }
    }
}