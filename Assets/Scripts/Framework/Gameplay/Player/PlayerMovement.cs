using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Player
{

    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _playerRigidBody;

        [SerializeField]
        private Vector2 _playerVelocity;
        private void Awake()
        {
            if(_playerRigidBody == null)
                _playerRigidBody = gameObject.GetComponent<Rigidbody>();
        }

        void Start()
        {
            SetMovementVector(new Vector2(10, 10));
        }

        private void SetMovementVector(Vector2 playerInput) => _playerVelocity = playerInput;
    }
}