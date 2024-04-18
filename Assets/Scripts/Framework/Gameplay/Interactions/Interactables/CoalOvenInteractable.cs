using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.CoalOvenSystem;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.DamageRepairManagement;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// A <see cref="Interactable"/> class responsible for handling the coal oven.
    /// </summary>
    [RequireComponent(typeof(TrainDamageable))]
    public class CoalOvenInteractable : Interactable, IRepairable
    {
        [SerializeField]
        private int _coalToAdd = 10;

        private TrainDamageable _damageable;

        private protected override void Awake()
        {
            base.Awake();

            _damageable = GetComponent<TrainDamageable>();
        }

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if(!base.Interact(interactor))
                return false;

            CoalOven.Instance.EnableOven();

            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="useableGrabbable"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        [Server]
        public override bool Interact(UseableGrabbable useableGrabbable)
        {
            CoalOven.Instance.AddCoal(_coalToAdd);

            useableGrabbable.OriginInteractor.UpdateInteractingTarget(useableGrabbable.OriginInteractor.Owner, null, false);
            useableGrabbable.Despawn();

            return base.Interact(useableGrabbable);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Repair() => _damageable.Repair();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public bool CanBeRepaired() => _damageable.CanBeRepaired();
    }
}