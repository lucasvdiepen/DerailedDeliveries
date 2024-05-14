using FishNet.Object;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DerailedDeliveries.Framework.InputParser
{
    /// <summary>
    /// A class responsible for parsing player input and invoking events based on the input.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputParser : NetworkBehaviour
    {
        /// <summary>
        /// An event invoked when the player pressed the interact button.
        /// </summary>
        public Action OnGrab;

        /// <summary>
        /// An event invoked when the player pressed the use button.
        /// </summary>
        public Action OnUse;

        /// <summary>
        /// An event invoked when the player gives input to move.
        /// Note that this event is only called when the input changes and not every frame.
        /// </summary>
        public Action<Vector2> OnMove;

        /// <summary>
        /// The direction the player is currently moving in.
        /// </summary>
        public Vector2 MoveDirection { get; private set; }

        private PlayerInput _playerInput;

        private void Awake() => _playerInput = GetComponent<PlayerInput>();

        public override void OnStartClient()
        {
            base.OnStartClient();

            if(!IsOwner)
                return;

            _playerInput.actions["Use"].performed += Use;
            _playerInput.actions["Move"].performed += Move;
            _playerInput.actions["Grab"].performed += Grab;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if(!IsOwner)
                return;

            _playerInput.actions["Use"].performed -= Use;
            _playerInput.actions["Move"].performed -= Move;
            _playerInput.actions["Grab"].performed -= Grab;
        }

        private void Move(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();

            OnMove?.Invoke(MoveDirection);
        }

        private void Grab(InputAction.CallbackContext context) => OnGrab?.Invoke();

        private void Use(InputAction.CallbackContext context) => OnUse?.Invoke();
    }
}