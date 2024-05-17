using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.CoalOvenSystem;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.DamageRepairManagement;
using DerailedDeliveries.Framework.Train;

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor)
            => base.CheckIfInteractable(interactor) && interactor.InteractingTarget is CoalGrabbable;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfUseable(Interactor interactor)
        {
            return IsInteractable && !IsOnCooldown
                && (interactor.InteractingTarget is HammerGrabbable && CanBeRepaired()
                    || interactor.InteractingTarget == null
                    && TrainEngine.Instance.EngineState == TrainEngineState.Inactive
                    && CoalOven.Instance.CoalAmount > 0.1
                    && _damageable.Health > 0);
        }

        [Server]
        private protected override bool Use(Interactor interactor)
        {
            if(!base.Use(interactor))
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