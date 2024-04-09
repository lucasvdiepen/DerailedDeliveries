using UnityEngine;
using FishNet;

using DerailedDeliveries.Framework.InputParser;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class responsible for handling the player's movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInputParser))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 2;

        [SerializeField]
        private float _maxSpeed = 2;

        [SerializeField]
        private float _rotationSpeed = 50;

        private PlayerInputParser _playerInputParser;
        private Rigidbody _rigidbody;
        private Vector2 _playerInput;

        private void Awake()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
            _playerInputParser = gameObject.GetComponent<PlayerInputParser>();
        }

        private void OnEnable()
        {
            _playerInputParser.OnMove += SetMovementVector;
            InstanceFinder.TimeManager.OnTick += UpdatePhysics;
        }

        private void OnDisable()
        {
            _playerInputParser.OnMove -= SetMovementVector;

            if (InstanceFinder.TimeManager != null)
                InstanceFinder.TimeManager.OnTick -= UpdatePhysics;
        }

        private void SetMovementVector(Vector2 playerInput) => _playerInput = playerInput;

        private void UpdatePhysics()
        {
            if (_playerInput == Vector2.zero)
                return;

            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;

            // Transform the input from local space to world space using the camera's forward direction.
            Vector3 playerInput = cameraForward * _playerInput.y + Camera.main.transform.right * _playerInput.x;
            playerInput = playerInput.normalized;

            UpdateVelocity(playerInput);
            UpdateRotation(playerInput);
        }

        private void UpdateRotation(Vector3 input)
        {
            gameObject.transform.rotation =
                    Quaternion.Slerp
                    (
                        gameObject.transform.rotation,
                        Quaternion.LookRotation(input),
                        Mathf.Clamp01(_rotationSpeed * Time.deltaTime)
                    );
        }

        private void UpdateVelocity(Vector3 input)
        {
            Vector3 newForce = _rigidbody.velocity + input * _speed;

            _rigidbody.AddForce(Vector3.ClampMagnitude(newForce, _maxSpeed));
        }
    }
}