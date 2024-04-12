using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    public class CoalPileInteractable : Interactable
    {
        [SerializeField]
        private GameObject _coalPrefab;

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