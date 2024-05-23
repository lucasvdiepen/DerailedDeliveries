using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.ParentingSystem;
using DerailedDeliveries.Framework.Utils.ObjectParenting;

namespace DerailedDeliveries.Framework.DamageRepairManagement.Damageables
{
    /// <summary>
    /// A <see cref="TrainDamageable"/> class which handles logic for the box.
    /// </summary>
    public class BoxDamageable : TrainDamageable
    {
        [SerializeField]
        private LayerMask _trainLayer;

        /// <summary>
        /// Gets whether the box is inside the train.
        /// </summary>
        public bool IsInsideTrain { get; private set; }

        [Server]
        private protected override void UpdateTimer()
        {
            if(!IsInsideTrain)
                return;

            base.UpdateTimer();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [Server]
        public override void TakeDamage()
        {
            if(!IsInsideTrain)
                return;

            base.TakeDamage();
        }

        private void OnTransformParentChanged()
        {
            if(ObjectParentUtils.TryGetObjectParent(gameObject, out ObjectParent objectParent))
            {
                if((_trainLayer.value & 1 << objectParent.gameObject.layer) != 0)
                {
                    IsInsideTrain = true;
                    return;
                }
            }

            IsInsideTrain = false;
        }
    }
}