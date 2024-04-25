using UnityEngine;
using FishNet.Component.Animating;

using DerailedDeliveries.Framework.Gameplay.Interactions;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.InputParser;
using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class responsible for handling the player's animations.
    /// </summary>
    [RequireComponent(typeof(PlayerInputParser), typeof(Interactor), typeof(Rigidbody))]
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private NetworkAnimator _networkAnimator;

        [SerializeField]
        private float _walkAnimationBaseVelocity = 3.5f;

        private PlayerInputParser _playerInputParser;
        private Interactor _interactor;
        private Rigidbody _rigidbody;
        private int _isWalkingAnimationHash;
        private int _interactAnimatioHash;
        private int _isCarryingAnimationHash;
        private int _walkSpeedMultiplierAnimationHash;

        private void Awake()
        {
            _playerInputParser = GetComponent<PlayerInputParser>();
            _interactor = GetComponent<Interactor>();
            _rigidbody = GetComponent<Rigidbody>();

            _isWalkingAnimationHash = Animator.StringToHash("IsWalking");
            _interactAnimatioHash = Animator.StringToHash("Interact");
            _isCarryingAnimationHash = Animator.StringToHash("IsCarrying");
            _walkSpeedMultiplierAnimationHash = Animator.StringToHash("WalkSpeedMultiplier");
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
        {
            // todo: set the correct animation speed. Maybe using velocity?
            _animator.SetFloat(_walkSpeedMultiplierAnimationHash, _rigidbody.velocity.magnitude / _walkAnimationBaseVelocity);
            _animator.SetBool(_isWalkingAnimationHash, moveDirection != Vector2.zero);
        }

        private void OnInteract(Interactable interactable)
        {
            if(interactable is Grabbable || interactable is CoalPileInteractable)
                return;

            _networkAnimator.SetTrigger(_interactAnimatioHash);
        }

        private void OnInteractingTargetChanged(Interactable interactable)
            =>_animator.SetBool(_isCarryingAnimationHash, interactable != null);
    }
}