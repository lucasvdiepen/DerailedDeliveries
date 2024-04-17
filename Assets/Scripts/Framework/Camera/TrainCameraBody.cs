using Cinemachine;
using UnityEngine;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// Class responsible for updating the train camera body on the horizontal axis.
    /// </summary>
    public class TrainCameraBody : MonoBehaviour
    {
        [SerializeField]
        private CinemachineTargetGroup _targetGroup;

        [SerializeField]
        private CinemachineVirtualCamera _cinemachineVirtualCamera;

        [SerializeField]
        private Transform _follingWagon;

        [SerializeField, Space]
        private float _lerpSpeed = 5;

        [SerializeField]
        private Vector3 _offset;

        private void Update()
            => UpdateCameraHorizontal();

        private void UpdateCameraHorizontal()
        {
            _targetGroup.transform.rotation = _follingWagon.rotation;

            Vector3 desiredPosition = _targetGroup.BoundingBox.center + _targetGroup.transform.right * _offset.x +
                                       _targetGroup.transform.up * _offset.y +
                                       _targetGroup.transform.forward * _offset.z;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, _lerpSpeed * Time.deltaTime);

            _cinemachineVirtualCamera.transform.position = transform.position;
        }
    }
}