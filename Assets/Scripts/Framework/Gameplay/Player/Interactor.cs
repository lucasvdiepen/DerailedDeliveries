using System.Collections.Generic;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions;
using DerailedDeliveries.Framework.InputParser;
using FishNet.Object;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class that is responsible for handling with in range Interactables for the player.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField]
        private List<Interactable> _interactables;

        [SerializeField]
        private Interactable _interactingTarget;

        [SerializeField]
        private Transform _grabbingAnchor;

        private PlayerInputParser _inputParser;

        private bool IsInteracting => _interactingTarget != null;

        private void Awake() => _inputParser = gameObject.GetComponent<PlayerInputParser>();

        private void OnEnable() => _inputParser.OnInteract += UseInteractable;

        private void OnDisable() => _inputParser.OnInteract -= UseInteractable;

        private void OnTriggerEnter(Collider collider)
        {
            GameObject target = collider.gameObject;

            if (target.TryGetComponent<Interactable>(out Interactable interactable))
            {
                if (_interactables.Contains(interactable) || IsInteracting)
                    return;

                _interactables.Add(interactable);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            GameObject target = collider.gameObject;

            if(target.TryGetComponent<Interactable>(out Interactable interactable))
            {
                if (!_interactables.Contains(interactable))
                    return;

                _interactables.Remove(interactable);
            }
        }

        private void UseInteractable()
        {
            if (_interactables.Count == 0 || IsInteracting)
            {
                if (IsInteracting)
                    _interactingTarget.Interact(this);

                return;
            }

            // TO DO: Priority checking for which interactable is most prio.

            InteractOnServer(_interactables[0]);
        }

        [ServerRpc]
        private void InteractOnServer(Interactable interactable)
        {
            interactable.Interact(this);
        }

        /// <summary>
        /// A function that sets the InteractingTarget of this Interactor.
        /// </summary>
        /// <param name="interactable">The current interacting target. If the interactor already had this
        /// reference set it will reset it.</param>
        public void SetInteractingTarget(Interactable interactable)
        {
            _interactingTarget = IsInteracting
                ? null
                : interactable;

            interactable.gameObject.transform.SetParent
            (
                IsInteracting
                ? _grabbingAnchor
                : null
            );

            if(IsInteracting)
                interactable.gameObject.transform.localPosition = Vector3.zero;
        }
    }
}