using DerailedDeliveries.Framework.GameManagement;
using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    public class ExitInteractable : Interactable
    {
        private protected override bool Use(Interactor interactor)
        {
            if (!base.Use(interactor))
                return false;

            GameManager.Instance.EndGame();

            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfUseable(Interactor interactor) => IsInteractable
            && !IsOnCooldown
            && interactor.InteractingTarget == null
            && TimerUpdater.Instance.VisitedStationsAmount > 1;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor)
        {
            return false;
        }
    }
}