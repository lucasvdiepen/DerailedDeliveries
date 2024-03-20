using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DerailedDeliveries.Framework.InputParser
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputParser : MonoBehaviour
    {
        public Action OnInteract;
        public Action<Vector2> OnMove;

        private PlayerInput _playerInput;

        private void Awake() => _playerInput = GetComponent<PlayerInput>();

        private void OnEnable()
        {
            _playerInput.actions["Move"].performed += Move;
            _playerInput.actions["Interact"].performed += Interact;
        }

        private void OnDisable()
        {
            _playerInput.actions["Move"].performed -= Move;
            _playerInput.actions["Interact"].performed -= Interact;
        }

        private void Move(InputAction.CallbackContext context) => OnMove?.Invoke(context.ReadValue<Vector2>());

        private void Interact(InputAction.CallbackContext context) => OnInteract?.Invoke();
    }
}