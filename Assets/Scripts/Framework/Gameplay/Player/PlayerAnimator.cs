using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.InputParser;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class responsible for handling the player's animations.
    /// </summary>
    [RequireComponent(typeof(PlayerInputParser), typeof(Interactor))]
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        private PlayerInputParser _playerInputParser;
        private Interactor _interactor;
        private int _isWalkingAnimationHash;
        private int _interactAnimatioHash;
        private int _isCarryingAnimationHash;

        private void Awake()
        {
            _playerInputParser = GetComponent<PlayerInputParser>();
            _interactor = GetComponent<Interactor>();

            _isWalkingAnimationHash = Animator.StringToHash("IsWalking");
            _interactAnimatioHash = Animator.StringToHash("Interact");
            _isCarryingAnimationHash = Animator.StringToHash("IsCarrying");
        }

        private void OnEnable()
        {
            _playerInputParser.OnMove += OnMove;
            _interactor.OnInteract += OnInteract;
            _interactor.OnInteractingTargetChanged += OnInteractingTargetChanged;
        }

        private void OnDisable()
        {
            _playerInputParser.OnMove -= OnMove;
            _interactor.OnInteract -= OnInteract;
            _interactor.OnInteractingTargetChanged -= OnInteractingTargetChanged;
        }

        private void OnMove(Vector2 moveDirection)
            =>_animator.SetBool(_isWalkingAnimationHash, moveDirection != Vector2.zero);

        private void OnInteract(Interactable interactable)
        {
            if(interactable is Grabbable)
                return;

            _animator.SetTrigger(_interactAnimatioHash);
        }

        private void OnInteractingTargetChanged(Interactable interactable)
            =>_animator.SetBool(_isCarryingAnimationHash, interactable != null);
    }
}