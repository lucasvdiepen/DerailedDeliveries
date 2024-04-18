using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// A <see cref="Interactable"/> class responsible for handling the coal pile.
    /// </summary>
    public class CoalPileInteractable : Interactable
    {
        [SerializeField]
        private GameObject _coalPrefab;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor)
            => base.CheckIfInteractable(interactor) && interactor.InteractingTarget == null;

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if(!base.Interact(interactor))
                return false;

            GameObject coalObject = Instantiate(_coalPrefab, Vector3.zero, Quaternion.identity);
            ServerManager.Spawn(coalObject);

            Grabbable coalInteractable = coalObject.GetComponent<Grabbable>();
            coalInteractable.InteractAsServer(interactor);

            return true;
        }
    }
}