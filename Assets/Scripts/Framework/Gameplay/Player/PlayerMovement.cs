using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class responsible for handling the player's movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _playerRigidBody;

        [SerializeField]
        private Vector2 _playerVelocity;

        [SerializeField]
        private float _playerSpeed = 2;

        private void Awake()
        {
            if(_playerRigidBody == null)
                _playerRigidBody = gameObject.GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            
        }

        private void SetMovementVector(Vector2 playerInput) => _playerVelocity = playerInput;

        private void FixedUpdate()
        {
            _playerRigidBody.velocity = new Vector3
            (
                _playerVelocity.x * _playerSpeed,
                0, 
                _playerVelocity.y * _playerSpeed
            );
        }
    }
}